using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Riber.Api.Tests.Fixtures;
using Riber.Application.Common;
using Riber.Application.Features.Auths.Commands.Login;
using Riber.Infrastructure.Persistence;
using Xunit;

namespace Riber.Api.Tests;

/*
 * A interface IAsyncLifeTime define dois métodos um para executar
 * antes dos testes e outro para executar depois dos testes rodarem.
 *
 * Já a interface IClassFixture<T> cria essa fixture 1 vez e compartilha entre os testes
 */
public abstract class IntegrationTestBase :
    IClassFixture<WebAppFixture>,
    IClassFixture<DatabaseFixture>,
    IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true, IncludeFields = true
    };

    private readonly WebAppFixture _webAppFixture;
    private WebApplicationFactory<Program> Factory => _webAppFixture.GetFactory();
    protected HttpClient Client { get; private set; } = null!;

    protected IntegrationTestBase(WebAppFixture webAppFixture, DatabaseFixture databaseFixture)
    {
        _webAppFixture = webAppFixture;
        _webAppFixture.SetConnectionString(databaseFixture.ConnectionString);
    }

    public async Task InitializeAsync()
    {
        Client = Factory.CreateClient();

        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync();
        await DatabaseFixture.ResetDatabaseAsync(context);
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        await Task.CompletedTask;
    }

    protected AppDbContext GetDbContext()
    {
        var scope = Factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    protected static async Task<Result<TResponse>?> ReadResultValueAsync<TResponse>(HttpResponseMessage response)
        where TResponse : class
    {
        var jsonString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Result<TResponse>>(jsonString, JsonOptions);
    }

    protected async Task AuthenticateAsync()
        => Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await GetTokenAsync());

    protected void ClearAuthentication()
        => Client.DefaultRequestHeaders.Authorization = null;

    private async Task<string> GetTokenAsync()
    {
        var loginResponse = await Client.PostAsJsonAsync("api/v1/auth/login",
            new LoginCommand("admin123", "Admin@123"));

        loginResponse.EnsureSuccessStatusCode();
        var result = await loginResponse.Content.ReadFromJsonAsync<Result<LoginCommandResponse>>();
        return result?.Value?.Token ?? string.Empty;
    }
}