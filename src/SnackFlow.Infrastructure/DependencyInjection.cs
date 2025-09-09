using Amazon.S3;
using Amazon.SimpleEmail;
using SnackFlow.Infrastructure.Persistence;
using SnackFlow.Infrastructure.Persistence.Interceptors;
using SnackFlow.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Quartz;
using Serilog;
using Serilog.Events;
using SnackFlow.Application.Abstractions.Schedulers;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.Abstractions.Services.Email;
using SnackFlow.Domain.Repositories;
using SnackFlow.Infrastructure.Jobs;
using SnackFlow.Infrastructure.Schedulers;
using SnackFlow.Infrastructure.Services;
using SnackFlow.Infrastructure.Services.Authentication;
using SnackFlow.Infrastructure.Services.AWS;
using SnackFlow.Infrastructure.Services.AWS.Email;
using SnackFlow.Infrastructure.Services.Local;

namespace SnackFlow.Infrastructure;

/// <summary>
/// Fornece métodos e configurações para configurar o container de injeção de dependência da camada de Infrastructure.
/// Esta classe pretende simplificar o processo de registro de serviços
/// e dependências necessárias para o funcionamento da aplicação.
/// </summary>
public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        logging.AddLogging();
        services.AddPersistence(configuration);
        services.AddRepositories();
        services.AddServices(configuration);
        services.AddAwsServices(configuration);
        services.AddBackgroundJobs(configuration);
        services.AddJsonConfiguration();
        services.AddHealthChecksConfiguration(configuration);
        services.AddSchedulersAndJobs();
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
            .WriteTo.File(
                path: "Common/Logs/app-.log",
                outputTemplate: output,
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Information,
                retainedFileCountLimit: 30)
            .WriteTo.File(
                path: "Common/Logs/errors-.log",
                outputTemplate: output,
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Error,
                retainedFileCountLimit: 90)
            .CreateLogger();

        logging.AddSerilog();
    }

    private static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options
                .UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
                .AddInterceptors(
                    new CaseInsensitiveInterceptor(),
                    new AuditInterceptor());
        });
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IInvitationRepository, InvitationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Transient
        services.AddTransient<ICertificateService, LocalCertificateService>();
        services.AddTransient<IEmailTemplateRender, EmailTemplateRender>();
        services.AddTransient<IEmailService, AmazonSesEmailService>();
        services.AddTransient<IPermissionDataService, PermissionDataService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<ITokenService, JwtTokenService>();
        services.AddTransient<IUserCreationService,  UserCreationService>();
        services.AddTransient<ICurrentUserService, CurrentUserService>();

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

    private static void AddSchedulersAndJobs(this IServiceCollection services)
    {
        services.AddTransient<SendingEmailJob>();
        services.AddScoped<IEmailScheduler, QuartzEmailScheduler>();
    }

    private static void AddHealthChecksConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(nameof(configuration));

        services
            .AddHealthChecks()
            .AddNpgSql(connectionString, name: "DefaultConnection");
    }

    private static void AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<QuartzOptions>(options =>
        {
            options.SchedulerName = "SnackFlowScheduler";
            options.Scheduling.IgnoreDuplicates = true;
            options.Scheduling.OverWriteExistingData = false;
        });
        
        services.AddQuartz(q =>
        {
            q.UsePersistentStore(configure =>
            {
                configure.PerformSchemaValidation = false;
                configure.UseProperties = true;
                configure.RetryInterval = TimeSpan.FromSeconds(15);
                configure.UseNewtonsoftJsonSerializer();
                configure.UsePostgres(postgres =>
                    postgres.ConnectionString =
                        configuration.GetConnectionString("DefaultConnection")
                        ?? throw new ArgumentNullException(nameof(configuration),
                            "QUARTZ - Connection string is null"));
            });

            q.AddJob<SendingEmailJob>(options => options
                .WithIdentity(new JobKey(nameof(SendingEmailJob)))
                .WithDescription("Envia de e-mails para notificar o usuário")
                .StoreDurably());
        });
        
        services.AddQuartzHostedService(options =>
            options.WaitForJobsToComplete = true);
    }
    
    private static void AddJsonConfiguration(this IServiceCollection _)
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            TypeNameHandling = TypeNameHandling.Auto
        };
    }
}