using SnackFlow.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SnackFlow.Infrastructure.Persistence.Factories;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../SnackFlow.Api"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder
            .UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b
                    .MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                    .MigrationsHistoryTable("__EFMigrationsHistory"))
            .AddInterceptors(
                new CaseInsensitiveInterceptor(),
                new AuditInterceptor());

        return new AppDbContext(optionsBuilder.Options);
    }
}