using System.Text.Json.Serialization;
using SnackFlow.Application;
using SnackFlow.Infrastructure;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SnackFlow.Api.Middlewares;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Infrastructure.Persistence;
using SnackFlow.Infrastructure.Persistence.Identity;
using SnackFlow.Infrastructure.Settings;

namespace SnackFlow.Api.Common.Api;

public static class BuilderExtension
{
    public static void AddPipeline(this WebApplicationBuilder builder)
    {
        builder.AddDocumentationApi();
        builder.AddDependencyInjection();
        builder.AddConfigurations();
        builder.AddMiddleware();
        builder.AddSecurity();
    }

    private static void AddDependencyInjection(this WebApplicationBuilder builder)
    {
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration, builder.Logging);
    }

    private static void AddMiddleware(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<CompanyRequiredMiddleware>();
        builder.Services.AddScoped<SecurityStampMiddleware>();
    }

    private static void AddConfigurations(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddEnvironmentVariables();
        builder.Services.AddControllers();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        builder.Services.AddMemoryCache();

        builder.Services
            .AddOptions<AccessTokenSettings>()
            .Bind(builder.Configuration.GetSection(nameof(AccessTokenSettings))) // <- Atribuí os valores
            .ValidateDataAnnotations() // ⇽ Pega as regras de validação
            .ValidateOnStart(); // ← Faz a validação, se não tiver válido, cancela a execução do programa

        builder.Services
            .AddOptions<RefreshTokenSettings>()
            .Bind(builder.Configuration.GetSection(nameof(RefreshTokenSettings)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services
            .Configure<ApiBehaviorOptions>(options
                => options.SuppressModelStateInvalidFilter = true)
            .Configure<JsonOptions>(options
                => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        // Para uso das políticas, consulte: docs/REQUEST-TIMEOUT.md
        builder.Services.AddRequestTimeouts(options =>
        {
            options.DefaultPolicy = new RequestTimeoutPolicy { Timeout = TimeSpan.FromMinutes(1) };
            options.AddPolicy("fast", TimeSpan.FromSeconds(15));
            options.AddPolicy("standard", TimeSpan.FromSeconds(30)); 
            options.AddPolicy("slow", TimeSpan.FromMinutes(1));
            options.AddPolicy("upload", TimeSpan.FromMinutes(5));
        });
    }

    private static void AddSecurity(this WebApplicationBuilder builder)
    {
        var accessToken =
            builder.Configuration.GetSection(nameof(AccessTokenSettings)).Get<AccessTokenSettings>()
            ?? throw new InvalidOperationException($"{nameof(AccessTokenSettings)} configuration not found");

        var refreshToken =
            builder.Configuration.GetSection(nameof(RefreshTokenSettings)).Get<RefreshTokenSettings>()
            ?? throw new InvalidOperationException($"{nameof(RefreshTokenSettings)} configuration not found");
        
        using var provider = builder.Services.BuildServiceProvider();
        var certificateService = provider.GetRequiredService<ICertificateService>();
        var accessCertificate = certificateService.LoadCertificate(
            accessToken.Key,
            accessToken.Password
        );
    
        var refreshCertificate = certificateService.LoadCertificate(
            refreshToken.Key,
            refreshToken.Password
        );
        
        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = nameof(AccessTokenSettings);
                options.DefaultChallengeScheme = nameof(AccessTokenSettings);
            })
            .AddJwtBearer(nameof(AccessTokenSettings), options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new X509SecurityKey(accessCertificate),
                    ValidateIssuer = true,
                    ValidIssuer = accessToken.Issuer,
                    ValidateAudience = true,
                    ValidAudience = accessToken.Audience,
                    ValidateLifetime = true, // ← Valida expiração
                    ClockSkew = TimeSpan.Zero, // ← Remove tolerância de tempo
                    RequireExpirationTime = true // ← Exige claim 'exp'
                };
            })
            .AddJwtBearer(nameof(RefreshTokenSettings), options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new X509SecurityKey(refreshCertificate),
                    ValidateIssuer = true,
                    ValidIssuer = refreshToken.Issuer,
                    ValidateAudience = true,
                    ValidAudience = refreshToken.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true
                };
            });
        
        builder.Services.AddAuthorization();
    }

    private static void AddDocumentationApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo 
            { 
                Title = "SnackFlow Documentation API", 
                Version = "v1" 
            });
            
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme", // ← Texto de ajuda
                Name = "Authorization", // ← Nome do header HTTP
                In = ParameterLocation.Header, // ← Aonde vai: no header da requisição
                Type = SecuritySchemeType.ApiKey, // ← Tipo: chave de API
                Scheme = "Bearer" // ← Esquema: Bearer token
            });
        });
    }
}