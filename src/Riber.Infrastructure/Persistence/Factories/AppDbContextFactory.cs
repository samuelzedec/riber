using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Riber.Infrastructure.Persistence.Interceptors;

namespace Riber.Infrastructure.Persistence.Factories;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string AppSettingsPath = "../../../../Riber.Api/appsettings.json";

    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(AppContext.BaseDirectory, AppSettingsPath), optional: false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b =>
                {
                    b.UseVector();
                    b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                        .MigrationsHistoryTable("__EFMigrationsHistory");
                })
            .AddInterceptors(new CaseInsensitiveInterceptor(), new AuditInterceptor())
            .EnableDetailedErrors()
            .EnableServiceProviderCaching();

        return new AppDbContext(optionsBuilder.Options);
    }
}