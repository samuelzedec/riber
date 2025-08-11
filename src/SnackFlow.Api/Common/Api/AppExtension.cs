using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SnackFlow.Api.Middlewares;

namespace SnackFlow.Api.Common.Api;

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
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        if (app.Environment.IsProduction())
            app.UseHttpsRedirection();
        
        app.UseRequestTimeouts();
    }

    private static void UseHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
    }
}