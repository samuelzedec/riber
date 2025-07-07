using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace SnackFlow.Api.Common.Api;

public static class AppExtension
{
    public static void UsePipeline(this WebApplication app)
    {
        app.UseConfigurations();
        app.UseSecurity();
        app.UseHealthChecks();
        app.MapControllers();
    }

    private static void UseSecurity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
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
        app.UseCors();
    }

    private static void UseHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
    }
}