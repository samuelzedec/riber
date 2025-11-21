using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Riber.Api.Authorizations.Permissions;
using Riber.Api.Common.Config;
using Riber.Api.Common.Transformers;
using Riber.Api.Middlewares;
using Riber.Application;
using Riber.Infrastructure;
using Riber.Infrastructure.Persistence;
using Riber.Infrastructure.Persistence.Identity;
using Riber.Infrastructure.Settings;

namespace Riber.Api.Common;

internal static class BuilderExtension
{
    extension(WebApplicationBuilder builder)
    {
        public void AddPipeline()
        {
            builder.AddJsonConfiguration();
            builder.AddDocumentationApi();
            builder.AddDependencyInjection();
            builder.AddConfigurations();
            builder.AddMiddleware();
            builder.AddSecurity();
        }

        private void AddDependencyInjection()
        {
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration, builder.Logging);
        }

        private void AddMiddleware()
        {
            builder.Services.AddScoped<SecurityStampMiddleware>();
        }

        private void AddConfigurations()
        {
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.AddServerHeader = false;
                options.ConfigureEndpointDefaults(endpoint
                    => endpoint.Protocols = HttpProtocols.Http1AndHttp2AndHttp3);
            });

            builder.Configuration.AddEnvironmentVariables();
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
            }).AddApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });
        }

        private void AddSecurity()
        {
            var accessToken =
                builder.Configuration.GetSection(nameof(AccessTokenSettings)).Get<AccessTokenSettings>()
                ?? throw new InvalidOperationException($"{nameof(AccessTokenSettings)} configuration not found");

            var refreshToken =
                builder.Configuration.GetSection(nameof(RefreshTokenSettings)).Get<RefreshTokenSettings>()
                ?? throw new InvalidOperationException($"{nameof(RefreshTokenSettings)} configuration not found");

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = nameof(AccessTokenSettings);
                    options.DefaultChallengeScheme = nameof(AccessTokenSettings);
                })
                .AddJwtBearer(nameof(AccessTokenSettings), options
                    => options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessToken.SecretKey)),
                        ValidateIssuer = true,
                        ValidIssuer = accessToken.Issuer,
                        ValidateAudience = true,
                        ValidAudience = accessToken.Audience,
                        ValidateLifetime = true, // ← Valida expiração
                        ClockSkew = TimeSpan.Zero, // ← Remove tolerância de tempo
                        RequireExpirationTime = true // ← Exige claim 'exp'
                    }
                )
                .AddJwtBearer(nameof(RefreshTokenSettings), options
                    => options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshToken.SecretKey)),
                        ValidateIssuer = true,
                        ValidIssuer = refreshToken.Issuer,
                        ValidateAudience = true,
                        ValidAudience = refreshToken.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        RequireExpirationTime = true
                    });

            builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            builder.Services.AddAuthorization();
        }

        private void AddDocumentationApi()
        {
            builder.Services.AddOpenApi(options
                => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
        }

        private void AddJsonConfiguration()
        {
            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.SerializerOptions.PropertyNameCaseInsensitive = true;
                options.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                options.SerializerOptions.AllowTrailingCommas = true;
                options.SerializerOptions.WriteIndented = true;
                options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            });

            builder.Services
                .AddControllers()
                .ConfigureInvalidModelStateResponse()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
                });
        }
    }
}