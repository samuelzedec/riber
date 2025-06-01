namespace ChefControl.Api.Common.Api;

public static class AppExtension
{
    public static void UsePipeline(this WebApplication app)
    {
        app.UseConfigurations();
        app.UseSecurity();
    }

    private static void UseSecurity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }

    private static void UseConfigurations(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseCors();
    }
}