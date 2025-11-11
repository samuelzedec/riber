using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Riber.Application.Common;

namespace Riber.Api.Extensions;

public static class HttpContextExtension
{
    private static JsonSerializerOptions JsonOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static async Task WriteErrorResponse(
        this HttpContext context,
        HttpStatusCode code,
        string message,
        Dictionary<string, string[]>? details = null,
        CancellationToken cancellationToken = default)
    {
        var response = Result.Failure<object>(message, code, details);
        var jsonResponse = JsonSerializer.Serialize(response, JsonOptions);

        context.Response.StatusCode = (int)code;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(jsonResponse, cancellationToken);
    }
}