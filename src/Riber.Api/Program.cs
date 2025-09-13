using Riber.Api.Common.Api;

var builder = WebApplication.CreateBuilder(args);
builder.AddPipeline();

var app = builder.Build();
app.UsePipeline();
app.Run();