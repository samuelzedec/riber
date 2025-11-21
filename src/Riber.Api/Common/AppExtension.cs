using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Riber.Api.Middlewares;
using Scalar.AspNetCore;

namespace Riber.Api.Common;

internal static class AppExtension
{
    extension(WebApplication app)
    {
        public void UsePipeline()
        {
            app.UseConfigurations();
            app.UseSecurity();
            app.UseMiddlewares();
            app.UseHealthChecks();
            app.MapControllers();
        }

        private void UseSecurity()
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        private void UseMiddlewares()
        {
            app.UseMiddleware<SecurityStampMiddleware>();
        }

        private void UseConfigurations()
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

        private void UseHealthChecks()
        {
            app.MapHealthChecks("/", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        }
    }
}