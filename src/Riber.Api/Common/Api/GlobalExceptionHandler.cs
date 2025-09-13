using Microsoft.AspNetCore.Diagnostics;
using Riber.Api.Extensions;
using Riber.Application.Exceptions;
using Riber.Domain.Exceptions;
using Layer = Riber.Application.Exceptions;

namespace Riber.Api.Common.Api;

/// <summary>
/// Trata exceções globais na aplicação registrando erros e formatando
/// uma resposta JSON padronizada para o cliente.
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Exception occurred: {ExceptionType} - {Message}", 
            exception.GetType().Name, exception.Message);
        
        (string? message, int statusCode, List<string>? errors) = exception switch
        {
            RequestTimeoutException timeoutEx => (timeoutEx.Message, timeoutEx.Code, null),
            ValidationException validationEx => (
                validationEx.Message, 
                validationEx.Code, 
                validationEx.Errors.Select(e => e.ErrorMessage).ToList()
            ),
            Layer.ApplicationException applicationEx => (applicationEx.Message, applicationEx.Code, null),
            DomainException domainEx => (domainEx.Message, StatusCodes.Status422UnprocessableEntity, null),
            _ => ("An unexpected error occurred", StatusCodes.Status500InternalServerError, null)
        };
        
        await httpContext.WriteUnauthorizedResponse(
            title: GetErrorCode(statusCode),
            message: message,
            code: statusCode,
            errors: errors?.ToArray() ?? []
        );
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