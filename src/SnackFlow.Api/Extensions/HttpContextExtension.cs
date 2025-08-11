using System.Text.Json;
using System.Text.Json.Serialization;
using SnackFlow.Application.Common;
using SnackFlow.Domain.Abstractions;

namespace SnackFlow.Api.Extensions;

public static class HttpContextExtension
{
    public static async Task WriteUnauthorizedResponse(
        this HttpContext context, 
        string title, 
        string message,
        int code,
        params string[]? errors)
    {
        var result = Result.Failure(new Error(title, message));
        var response = new
        {
            isSuccess = result.IsSuccess,
            isFailure = result.IsFailure,
            error = new
            {
                code = result.Error.Code,
                message = result.Error.Message,
                details = errors ?? []
            }
        };

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        context.Response.StatusCode = code;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(jsonResponse);
    }
}