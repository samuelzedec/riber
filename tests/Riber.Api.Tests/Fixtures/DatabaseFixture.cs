using Microsoft.EntityFrameworkCore;
using Riber.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace Riber.Api.Tests.Fixtures;

public sealed class DatabaseFixture : IAsyncLifetime
{
    #region Fields

    private PostgreSqlContainer _dbContainer = null!;
    public string ConnectionString { get; private set; } = null!;

    #endregion

    #region Methods

    public async Task InitializeAsync()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("pgvector/pgvector:pg18")
            .WithDatabase("riber_test")
            .WithUsername("postgres")
            .WithPassword("root")
            .Build();

        await _dbContainer.StartAsync();
        ConnectionString = _dbContainer.GetConnectionString();
    }

    public async Task DisposeAsync()
        => await _dbContainer.DisposeAsync();

    public static async Task ResetDatabaseAsync(AppDbContext context)
    {
        await context.Database.ExecuteSqlRawAsync("SET session_replication_role = 'replica';");

        var seedIds = new Dictionary<string, string[]>
        {
            ["company"] = ["ba99e7c2-f824-490d-a0fb-6dae78cc0163"],
            ["user"] = ["d670c05b-461d-4f06-8b05-288eb4671758", "a1c4f6e2-2d6e-4f3b-8e3a-3c9e5f7b8a1b"],
            ["aspnet_user"] = ["e6fba186-c1e7-4083-90a6-966c421720e5", "c819a3a8-37e1-46ab-94df-2186e0170bd1"]
        };

        var tablesToSkipCompletely = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            "application_permission",
            "aspnet_role",
            "aspnet_role_claim",
            "aspnet_user_role"
        };

        var tables = await context.Database
            .SqlQueryRaw<string>("""
                                     SELECT tablename 
                                     FROM pg_tables 
                                     WHERE schemaname='public' 
                                     AND tablename != '__EFMigrationsHistory';
                                 """)
            .ToListAsync();

#pragma warning disable EF1002
        foreach (var table in tables.Where(table => !tablesToSkipCompletely.Contains(table)))
        {
            if (seedIds.TryGetValue(table, out string[]? id))
            {
                var ids = string.Join("','", id);
                await context.Database.ExecuteSqlRawAsync(
                    $"DELETE FROM \"{table}\" WHERE id NOT IN ('{ids}');"
                );
            }
            else
            {
                await context.Database.ExecuteSqlRawAsync(
                    $"TRUNCATE TABLE \"{table}\" RESTART IDENTITY CASCADE;"
                );
            }
        }
#pragma warning restore EF1002
        await context.Database.ExecuteSqlRawAsync("SET session_replication_role = 'origin';");
    }

    #endregion
}