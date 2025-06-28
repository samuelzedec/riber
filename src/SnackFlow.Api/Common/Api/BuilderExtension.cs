using System.Text.Json.Serialization;
using SnackFlow.Application;
using SnackFlow.Infrastructure;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SnackFlow.Application.Configuration;

namespace SnackFlow.Api.Common.Api;

public static class BuilderExtension
{
    public static void AddPipeline(this WebApplicationBuilder builder)
    {
        builder.AddDependencyInjection();
        builder.AddConfigurations();
        builder.AddSecurity();
    }

    private static void AddDependencyInjection(this WebApplicationBuilder builder)
    {
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration, builder.Logging);
    }

    private static void AddConfigurations(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Configuration.AddEnvironmentVariables();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services
            .AddOptions<AccessTokenSettings>()
            .Bind(builder.Configuration.GetSection(nameof(AccessTokenSettings))) // <- Atribuí os valores
            .ValidateDataAnnotations() // ⇽ Pega as regras de validação
            .ValidateOnStart(); // ← Faz as validação, se não tiver válido, cancela a execução do programa

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

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = accessToken.Key;
                options.DefaultChallengeScheme = accessToken.Key;
            })
            .AddJwtBearer(accessToken.Key, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new X509SecurityKey(accessToken.GenerateCertificate()),
                    ValidateIssuer = true,
                    ValidIssuer = accessToken.Issuer,
                    ValidateAudience = true,
                    ValidAudience = accessToken.Audience,
                    ValidateLifetime = true, // ← Valida expiração
                    ClockSkew = TimeSpan.Zero, // ← Remove tolerância de tempo
                    RequireExpirationTime = true // ← Exige claim 'exp'
                };
            })
            .AddJwtBearer(refreshToken.Key, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new X509SecurityKey(refreshToken.GenerateCertificate()),
                    ValidateIssuer = true,
                    ValidIssuer = refreshToken.Issuer,
                    ValidateAudience = true,
                    ValidAudience = refreshToken.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true
                };
            });
    }
}