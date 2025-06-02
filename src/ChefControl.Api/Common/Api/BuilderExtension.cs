using System.Text.Json.Serialization;
using ChefControl.Application;
using ChefControl.Application.SharedContext.Common;
using ChefControl.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ChefControl.Api.Common.Api;

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
        builder.Configuration.AddEnvironmentVariables();

        builder.Services
            .AddOptions<AccessToken>()
            .Bind(builder.Configuration.GetSection("AccessToken")) // <- Atribuí os valores
            .ValidateDataAnnotations() // <- Pega as regras de validação
            .ValidateOnStart(); // <- Faz as validação, se não tiver válido, cancela a execução do programa

        builder.Services
            .AddOptions<RefreshToken>()
            .Bind(builder.Configuration.GetSection("RefreshToken"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services
            .Configure<ApiBehaviorOptions>(options
                => options.SuppressModelStateInvalidFilter = true)
            .Configure<JsonOptions>(options
                => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
    }

    private static void AddSecurity(this WebApplicationBuilder builder)
    {
        var accessToken =
            builder.Configuration.GetSection("AccessToken").Get<AccessToken>()
            ?? throw new InvalidOperationException("AccessToken configuration not found");

        var refreshToken =
            builder.Configuration.GetSection("RefreshToken").Get<RefreshToken>()
            ?? throw new InvalidOperationException("RefreshToken configuration not found");

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