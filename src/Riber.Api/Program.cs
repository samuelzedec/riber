using Microsoft.EntityFrameworkCore;
using Riber.Api.Common;
using Riber.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.AddPipeline();

var app = builder.Build();
app.UsePipeline();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
}

await app.RunAsync();

namespace Riber.Api
{
    /// <summary>
    /// Ponto de entrada da aplicação Riber API.
    /// Esta classe é utilizada como referência de tipo para testes de integração.
    /// </summary>
    public partial class Program { protected Program() { }}
}