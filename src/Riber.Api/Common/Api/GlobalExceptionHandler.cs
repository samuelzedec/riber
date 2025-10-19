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
            code: statusCode,
            messages: messages
        );
        return true;
    }
}