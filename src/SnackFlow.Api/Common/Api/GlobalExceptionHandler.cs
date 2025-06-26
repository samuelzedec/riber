using System.Text.Json;
using SnackFlow.Application.SharedContext.Exceptions;
using SnackFlow.Application.SharedContext.Results;
using SnackFlow.Domain.SharedContext.Abstractions;
using SnackFlow.Domain.SharedContext.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using ApplicationException = SnackFlow.Application.SharedContext.Exceptions.ApplicationException;

namespace SnackFlow.Api.Common.Api;

/// <summary>
/// Trata exceções globais na aplicação registrando erros e formatando
/// uma resposta JSON padronizada para o cliente.
/// </summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Exception occurred: {ExceptionType} - {Message}", 
            exception.GetType().Name, exception.Message);
        
        var (message, statusCode, errors) = exception switch
        {
            RequestTimeoutException timeoutEx => (timeoutEx.Message, timeoutEx.Code, null),
            ValidationException validationEx => (
                validationEx.Message, 
                validationEx.Code, 
                validationEx.Errors.Select(e => e.ErrorMessage).ToList()
            ),
            ApplicationException applicationEx => (applicationEx.Message, applicationEx.Code, null),
            DomainException domainEx => (domainEx.Message, StatusCodes.Status422UnprocessableEntity, null),
            _ => ("An unexpected error occurred", StatusCodes.Status500InternalServerError, null)
        };
        
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        var result = Result.Failure(new Error(GetErrorCode(statusCode), message));
        var response = new
        {
            isSuccess = result.IsSuccess,
            isFailure = result.IsFailure,
            error = new
            {
                code = result.Error.Code,
                message = result.Error.Message,
                details = errors
            }
        };
        
        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
        
        await httpContext.Response.WriteAsync(jsonResponse, cancellationToken);
        return true;
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