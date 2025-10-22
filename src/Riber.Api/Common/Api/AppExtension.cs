using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Riber.Api.Middlewares;
using Scalar.AspNetCore;

namespace Riber.Api.Common.Api;

public static class AppExtension
{
    public static void UsePipeline(this WebApplication app)
    {
        app.UseConfigurations();
        app.UseSecurity();
        app.UseMiddlewares();
        app.UseHealthChecks();
        app.MapControllers();
    }

    private static void UseSecurity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
    
    private static void UseMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<SecurityStampMiddleware>();
    }

    private static void UseConfigurations(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("Referrer-Policy", "no-referrer");
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            await next();
        });
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }
        
        if (app.Environment.IsProduction())
            app.UseHttpsRedirection();
        
        app.UseRequestTimeouts();
    }

    private static void UseHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
    }
}