using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using SnackFlow.Application;
using SnackFlow.Infrastructure;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SnackFlow.Api.Authorizations.Permissions;
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

        builder.Services.AddApiVersioning(options =>
        {
            // Define a versão padrão da API (1.0)
            options.DefaultApiVersion = new ApiVersion(1, 0);
            // Se o cliente não especificar a versão, assume a versão padrão
            options.AssumeDefaultVersionWhenUnspecified = true;
            // Define COMO o cliente pode especificar a versão
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(), // via URL: /api/v1/company
                new QueryStringApiVersionReader("version"), // via query: ?version=1.0
                new HeaderApiVersionReader("X-Version") // via header: X-Version: 1.0
            );
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
        
        builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
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
                Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme 
                    {
                        Reference = new OpenApiReference 
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "Bearer",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
        });
    }
}