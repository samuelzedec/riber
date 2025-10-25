using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Riber.Application.Common;
using Riber.Domain.Abstractions;

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
        Dictionary<string, string[]> details)
    {
        var response = details.Count == 0
            ? Result.Failure<object>(message, code)
            : Result.Failure<object>(details, code);

        var jsonResponse = JsonSerializer.Serialize(response, JsonOptions);
        context.Response.StatusCode = (int)code;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(jsonResponse);
    }
}