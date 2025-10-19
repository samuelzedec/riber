using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Riber.Application.Common;
using Riber.Application.Features.Auths.Commands.Login;
using Riber.Infrastructure.Persistence;
using Riber.Infrastructure.Persistence.Interceptors;
using Testcontainers.PostgreSql;
using Xunit;

namespace Riber.Api.Tests;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    #region Fields

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:14.1-alpine")
        .WithDatabase("riber_test")
        .WithUsername("postgres")
        .WithPassword("root")
        .Build();

    private WebApplicationFactory<Program> Factory { get; set; } = null!;
    private HttpClient Client { get; set; } = null!;

    #endregion

    #region Methods

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder
                .ConfigureTestServices(services =>
                {
                    // Irá remover as injeções inseridas no contexto de banco de dados
                    services.RemoveAll<DbContextOptions<AppDbContext>>();
                    services.RemoveAll<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService>();

                    // Inserindo as injeções de banco de dados para testes
                    services.AddDbContext<AppDbContext>(options =>
                        options
                            .UseNpgsql(_dbContainer.GetConnectionString())
                            .AddInterceptors(new CaseInsensitiveInterceptor(), new AuditInterceptor())
                    );
                }));

        Client = Factory.CreateClient();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await Factory.DisposeAsync();
        Client.Dispose();
    }

    protected AppDbContext GetDbContext()
    {
        var scope = Factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    protected async Task ResetDatabaseAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        /*
         * Isso fará que seja desabilitado no postgres temporiamente as restrições de integridade
         * como: foreign key, unique key, check, etc.
         */
        await context.Database.ExecuteSqlRawAsync("SET session_replication_role = 'replica';");
        var tables = await context.Database
            .SqlQueryRaw<string>("""
                                      SELECT tablename 
                                      FROM pg_tables 
                                      WHERE schemaname='public' 
                                      AND tablename != '__EFMigrationsHistory';
                                 """)
            .ToListAsync();

#pragma warning disable EF1002
        foreach (var table in tables)
            await context.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE \"{table}\" RESTART IDENTITY CASCADE;");
#pragma warning restore EF1002

        await context.Database.ExecuteSqlRawAsync("SET session_replication_role = 'origin';");
    }

    protected async Task AuthenticateAsync()
        => Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await GetTokenAsync());

    protected void ClearAuthentication()
        => Client.DefaultRequestHeaders.Authorization = null;

    private async Task<string> GetTokenAsync()
    {
        var loginResponse = await Client.PostAsJsonAsync("api/v1/auth/login",
            new { Username = "admin123", Password = "Admin@123" });

        loginResponse.EnsureSuccessStatusCode();
        var result = await loginResponse.Content.ReadFromJsonAsync<Result<LoginCommandResponse>>();
        return result?.Value?.Token ?? string.Empty;
    }

    #endregion
}