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
        
        (int statusCode, string[] messages) = exception switch
        {
            RequestTimeoutException timeoutEx => (timeoutEx.Code, [timeoutEx.Message]),
            ValidationException validationEx => (ValidationException.Code, validationEx.Messages.Select(e => e.ErrorMessage).ToArray()),
            Layer.ApplicationException applicationEx => (applicationEx.Code, [applicationEx.Message]),
            DomainException domainEx => (StatusCodes.Status422UnprocessableEntity, [domainEx.Message]),
            _ => (StatusCodes.Status500InternalServerError, ["Erro inesperado no servidor!"])
        };
        
        await httpContext.WriteUnauthorizedResponse(
            title: GetErrorCode(statusCode),
            code: statusCode,
            messages: messages
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