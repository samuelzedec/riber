using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Riber.Infrastructure.Messaging.Consumers;
using Riber.Infrastructure.Persistence;
using Riber.Infrastructure.Persistence.Interceptors;
using Xunit;

namespace Riber.Api.Tests.Fixtures;

public sealed class WebAppFixture : IAsyncLifetime
{
    private WebApplicationFactory<Program>? _factory;
    private string ConnectionString { get; set; } = null!;

    public async Task InitializeAsync()
        => await Task.CompletedTask;

    public async Task DisposeAsync()
    {
        if (_factory is not null)
            await _factory.DisposeAsync();
    }

    public void SetConnectionString(string connectionString)
        => ConnectionString = connectionString;

    public WebApplicationFactory<Program> GetFactory()
    {
        if (_factory != null)
            return _factory;

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
                builder.ConfigureAppConfiguration((_, config) =>
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["ConnectionStrings:DefaultConnection"] = ConnectionString,
                        ["Quartz:quartz.dataSource.default.connectionString"] = ConnectionString,
                    }!));

                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<DbContextOptions<AppDbContext>>();
                    services.AddDbContext<AppDbContext>(options =>
                        options
                            .UseNpgsql(ConnectionString)
                            .AddInterceptors(new CaseInsensitiveInterceptor(), new AuditInterceptor())
                    );
                    services.RemoveAll<IHostedService>();

                    services.RemoveAll<IBusControl>();
                    services.RemoveAll<IPublishEndpoint>();
                    services.RemoveAll<ISendEndpointProvider>();

                    var massTransitDescriptors = services
                        .Where(d => d.ServiceType.FullName?.Contains("MassTransit") ?? false)
                        .ToList();

                    foreach (var descriptor in massTransitDescriptors)
                        services.Remove(descriptor);


                    services.AddMassTransit(busConfigurator =>
                    {
                        busConfigurator.AddConsumer<SendEmailMessageConsumer>();
                        busConfigurator.AddConsumer<ProductImageCreationFailedMessageConsumer>();
                        busConfigurator.AddConsumer<GenerateProductEmbeddingsMessageConsumer>();

                        busConfigurator.UsingInMemory((context, configurator) =>
                        {
                            configurator.ConfigureEndpoints(context);
                        });
                    });
                });
            });
        return _factory;
    }
}