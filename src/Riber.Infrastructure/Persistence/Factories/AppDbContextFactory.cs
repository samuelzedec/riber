using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Riber.Infrastructure.Persistence.Interceptors;

namespace Riber.Infrastructure.Persistence.Factories;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Riber.Api"))
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