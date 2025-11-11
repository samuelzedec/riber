using Microsoft.AspNetCore.Diagnostics;
using Riber.Api.Extensions;
using Riber.Application.Exceptions;
using Riber.Domain.Exceptions;
using System.Net;
using Layer = Riber.Application.Exceptions;

namespace Riber.Api.Common.Api;

/// <summary>
/// Trata exceções globais na aplicação registrando erros e formatando
/// uma resposta JSON padronizada para o cliente.
/// </summary>
public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Exception occurred: {ExceptionType} - {Message}",
            exception.GetType().Name, exception.Message);

        (HttpStatusCode statusCode, string message, Dictionary<string, string[]>? details) = MapExceptionToError(exception);
        await httpContext.WriteErrorResponse(statusCode, message, details, cancellationToken);
        return true;
    }

    private static (HttpStatusCode, string, Dictionary<string, string[]>?) MapExceptionToError(Exception exception)
        => exception switch
        {
            RequestTimeoutException timeoutEx => (timeoutEx.Code, timeoutEx.Message, null),
            ValidationException validationEx => (HttpStatusCode.BadRequest, "Dados Inválidos.", validationEx.Details),
            Layer.ApplicationException applicationEx => (applicationEx.Code, applicationEx.Message, null),
            DomainException domainEx => (HttpStatusCode.UnprocessableContent, domainEx.Message, null),
            _ => (HttpStatusCode.InternalServerError, "Erro inesperado no servidor.", null)
        };
}