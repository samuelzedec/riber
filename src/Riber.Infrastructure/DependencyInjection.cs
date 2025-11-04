using Amazon.S3;
using Amazon.SimpleEmail;
using Anthropic.SDK;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OllamaSharp;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Quartz;
using Riber.Application.Abstractions.Messaging;
using Serilog;
using Serilog.Events;
using Riber.Application.Abstractions.Services;
using Riber.Application.Abstractions.Services.AI;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Abstractions.Services.Email;
using Riber.Application.Configurations;
using Riber.Application.Exceptions;
using Riber.Domain.Entities;
using Riber.Domain.Repositories;
using Riber.Infrastructure.BackgroundJobs;
using Riber.Infrastructure.Messaging;
using Riber.Infrastructure.Messaging.Consumers;
using Riber.Infrastructure.Persistence;
using Riber.Infrastructure.Persistence.Interceptors;
using Riber.Infrastructure.Persistence.Models.Embeddings;
using Riber.Infrastructure.Persistence.Repositories;
using Riber.Infrastructure.Schedulers;
using Riber.Infrastructure.Services.AI;
using Riber.Infrastructure.Services.AI.Models;
using Riber.Infrastructure.Services.Authentication;
using Riber.Infrastructure.Services.Authentication.Identity;
using Riber.Infrastructure.Services.AWS;
using Riber.Infrastructure.Services.AWS.Email;
using Riber.Infrastructure.Services.Local;

namespace Riber.Infrastructure;

/// <summary>
/// Fornece métodos e configurações para configurar o container de injeção de dependência da camada de Infrastructure.
/// Esta classe pretende simplificar o processo de registro de serviços
/// e dependências necessárias para o funcionamento da aplicação.
/// </summary>
public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration,
        ILoggingBuilder logging)
    {
        var defaultConnection = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InternalException("Default connection string is not set.");

        logging.AddLogging();
        services.AddPersistence(defaultConnection);
        services.AddRepositories();
        services.AddServices(configuration);
        services.AddAwsServices(configuration);
        services.AddBackgroundJobs(defaultConnection);
        services.AddHealthChecksConfiguration(defaultConnection);
        services.AddTelemetry();
        services.AddAi(configuration);
        services.AddMessaging(configuration);
    }

    private static void AddLogging(this ILoggingBuilder logging)
    {
        const string output =
            "[{Timestamp:dd/MM/yyyy HH:mm:ss}] {Level:u3} | {SourceContext} | {Message:lj}{NewLine}{Exception}";
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: output)
            .WriteTo.Logger(lc => lc
                .Filter.ByExcluding(evt => evt.Level >= LogEventLevel.Error)
                .WriteTo.File(
                    path: "Common/Logs/app-.log",
                    outputTemplate: output,
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    retainedFileCountLimit: 30))
            .WriteTo.File(
                path: "Common/Logs/errors-.log",
                outputTemplate: output,
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Error,
                retainedFileCountLimit: 90)
            .CreateLogger();

        logging.AddSerilog();
    }

    private static void AddPersistence(this IServiceCollection services, string defaultConnection)
    {
        services.AddDbContext<AppDbContext>(options => options
            .UseNpgsql(defaultConnection, b =>
            {
                b.UseVector();
                b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                    .MigrationsHistoryTable("__EFMigrationsHistory");
            })
            .AddInterceptors(new CaseInsensitiveInterceptor(), new AuditInterceptor()));
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IInvitationRepository, InvitationRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IEmailTemplateRender, EmailTemplateRender>();
        services.AddTransient<IEmailService, AmazonSESService>();
        services.AddTransient<IPermissionDataService, PermissionDataService>();
        services.AddTransient<ITokenService, JwtTokenService>();
        services.AddTransient<IUserCreationService, UserCreationService>();
        services.AddTransient<ICurrentUserService, CurrentUserService>();
        services.AddTransient<IUserQueryService, UserQueryService>();
        services.AddTransient<IUserManagementService, UserManagementService>();
        services.AddTransient<IRoleManagementService, RoleManagementService>();
        services.AddTransient<IAuthenticationService, AuthenticationService>();
        services.AddTransient<IEmbeddingsService, EmbeddingsService>();
        services.AddTransient<IAiModelService<ProductEmbeddingsModel, Product>, AiProductService>();
        services.AddScoped<UserMappingService>();
        services.AddSingleton<IEmailConcurrencyService, EmailConcurrencyService>();

        if (configuration["ASPNETCORE_ENVIRONMENT"] == "Production")
            services.AddTransient<IImageStorageService, AmazonS3Service>();
        else
            services.AddTransient<IImageStorageService, LocalImageStorageService>();
    }

    private static void AddAwsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonSimpleEmailService>();
        services.AddAWSService<IAmazonS3>();
    }

    private static void AddHealthChecksConfiguration(this IServiceCollection services, string defaultConnection)
    {
        services
            .AddHealthChecks()
            .AddNpgSql(defaultConnection, name: nameof(defaultConnection));
    }

    private static void AddBackgroundJobs(this IServiceCollection services, string defaultConnection)
    {
        services.AddTransient<CleanupImageBucketJob>();

        services.Configure<QuartzOptions>(options =>
        {
            options.SchedulerName = "RiberScheduler";
            options.Scheduling.IgnoreDuplicates = true;
            options.Scheduling.OverWriteExistingData = false;
        });

        services.AddQuartz(quartz =>
        {
            quartz.UsePersistentStore(configure =>
            {
                configure.PerformSchemaValidation = false;
                configure.UseProperties = true;
                configure.RetryInterval = TimeSpan.FromSeconds(15);
                configure.UseNewtonsoftJsonSerializer();
                configure.UsePostgres(postgres => postgres.ConnectionString = defaultConnection);
            });

            CleanupImageBucketScheduler.Configure(quartz);
        });

        services.AddQuartzHostedService(options =>
            options.WaitForJobsToComplete = true);
    }

    private static void AddTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(DiagnosticsConfig.ActivitySourceName))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter())
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddQuartzInstrumentation() // -> Beta
                .AddOtlpExporter())
            .WithLogging(logging => logging
                .AddOtlpExporter());
    }

    private static void AddAi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(_ => new AnthropicClient(configuration["Anthropic:ApiKey"]));
        services.AddSingleton<IChatClient>(sp =>
        {
            var anthropicClient = sp.GetRequiredService<AnthropicClient>();
            return new ChatClientBuilder(anthropicClient.Messages).UseFunctionInvocation().Build();
        });

        services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(_ =>
            new OllamaApiClient(new Uri(configuration["Ollama:ApiUri"]!), configuration["Ollama:EmbeddingModel"]!));
    }

    private static void AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IMessagePublisher, MassTransitMessagePublisher>();
        services.AddTransient<SendEmailMessageConsumer>();
        services.AddTransient<ProductImageCreationFailedMessageConsumer>();
        services.AddTransient<GenerateProductEmbeddingsMessageConsumer>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddConsumer<SendEmailMessageConsumer>();
            busConfigurator.AddConsumer<ProductImageCreationFailedMessageConsumer>();
            busConfigurator.AddConsumer<GenerateProductEmbeddingsMessageConsumer>();
            busConfigurator.UsingRabbitMq((context, configurator) =>
            {
                configurator.Host(new Uri(configuration["RabbitMQ:Host"]!), host =>
                {
                    host.Username(configuration["RabbitMQ:UserName"]!);
                    host.Password(configuration["RabbitMQ:Password"]!);
                });
                configurator.ConfigureEndpoints(context);
            });
        });
    }
}