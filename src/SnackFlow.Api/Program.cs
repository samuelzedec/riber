using SnackFlow.Api.Common.Api;

var builder = WebApplication.CreateBuilder(args);
await builder.AddPipeline();

var app = builder.Build();
app.UsePipeline();
app.Run();