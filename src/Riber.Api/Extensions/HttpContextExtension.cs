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
        int code,
        params string[] messages)
    {
        var jsonResponse = JsonSerializer.Serialize(
            Result.Failure<object>(new Error(GetErrorCode(code), messages)), JsonOptions);

        context.Response.StatusCode = code;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(jsonResponse);
    }

    private static string GetErrorCode(int statusCode) => statusCode switch
    {
        400 => "BAD_REQUEST",
        401 => "UNAUTHORIZED",
        404 => "NOT_FOUND",
        408 => "REQUEST_TIMEOUT",
        409 => "CONFLICT",
        422 => "UNPROCESSABLE_ENTITY",
        500 => "INTERNAL_SERVER_ERROR",
        _ => "APPLICATION_ERROR"
    };
}