using Microsoft.EntityFrameworkCore;
using Riber.Api.Common.Api;
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

app.Run();