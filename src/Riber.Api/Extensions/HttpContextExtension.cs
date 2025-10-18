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

    public static async Task WriteUnauthorizedResponse(
        this HttpContext context,
        string title,
        int code,
        params string[] messages)
    {
        var result = Result.Failure(new Error(title, messages));
        var response = new
        {
            isSuccess = result.IsSuccess,
            error = new { code = result.Error.Code, messages }
        };

        var jsonResponse = JsonSerializer.Serialize(response, JsonOptions);
        context.Response.StatusCode = code;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(jsonResponse);
    }
}