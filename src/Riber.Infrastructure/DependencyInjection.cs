using Amazon.S3;
using Amazon.SimpleEmail;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Quartz;
using Riber.Application.Abstractions.Dispatchers;
using Serilog;
using Serilog.Events;
using Riber.Application.Abstractions.Services;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Abstractions.Services.Email;
using Riber.Application.Configurations;
using Riber.Application.Exceptions;
using Riber.Domain.Repositories;
using Riber.Infrastructure.BackgroundJobs;
using Riber.Infrastructure.Dispatchers;
using Riber.Infrastructure.Persistence;
using Riber.Infrastructure.Persistence.Interceptors;
using Riber.Infrastructure.Persistence.Repositories;
using Riber.Infrastructure.Schedulers;
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
        services.AddDispachersAndJobs();
        services.AddTelemetry();
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
            .UseNpgsql(defaultConnection, b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
            .AddInterceptors(
                new CaseInsensitiveInterceptor(),
                new AuditInterceptor()));
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
        // Transient & Scoped
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
        services.AddScoped<UserMappingService>();

        if (configuration["ASPNETCORE_ENVIRONMENT"] == "Development")
            services.AddTransient<IImageStorageService, LocalImageStorageService>();
        else
            services.AddTransient<IImageStorageService, AmazonS3Service>();

        // Singleton
        services.AddSingleton<IEmailConcurrencyService, EmailConcurrencyService>();
    }

    private static void AddAwsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonSimpleEmailService>();
        services.AddAWSService<IAmazonS3>();
    }

    private static void AddDispachersAndJobs(this IServiceCollection services)
    {
        // Background jobs
        services.AddTransient<SendingEmailJob>();
        services.AddTransient<CleanupImageBucketJob>();
        services.AddTransient<DeleteImageFromStorageJob>();

        // Dispatchers
        services.AddScoped<IEmailDispatcher, EmailDispatcher>();
        services.AddScoped<IDeleteImageFromStorageDispatcher, DeleteImageFromStorageDispatcher>();
    }

    private static void AddHealthChecksConfiguration(this IServiceCollection services, string defaultConnection)
    {
        services
            .AddHealthChecks()
            .AddNpgSql(defaultConnection, name: nameof(defaultConnection));
    }

    private static void AddBackgroundJobs(this IServiceCollection services, string defaultConnection)
    {
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
            SendingEmailScheduler.Configure(quartz);
            DeleteImageFromStorageScheduler.Configure(quartz);
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
}