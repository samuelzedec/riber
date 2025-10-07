using System.Diagnostics;
using Mediator;
using Microsoft.Extensions.Logging;
using Riber.Application.Configurations;
using Riber.Application.Exceptions;

namespace Riber.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IMessage
{
    public async ValueTask<TResponse> Handle(
        TRequest message,
        MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        var messageName = message.GetType().Name;
        var stopwatch = Stopwatch.StartNew();

        using var activity = DiagnosticsConfig
            .ActivitySource
            .StartActivity($"Mediator.{messageName}");

        try
        {
            logger.LogInformation("Handling: {RequestFullName}", messageName);
            var result = await next(message, cancellationToken);

            stopwatch.Stop();
            activity?.SetTag("duration_ms", stopwatch.ElapsedMilliseconds);
            activity?.SetStatus(ActivityStatusCode.Ok);

            logger.LogInformation(
                "{RequestName} completed in {ElapsedMs}ms",
                messageName,
                stopwatch.ElapsedMilliseconds
            );

            return result;
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Error, "Request cancelled");

            logger.LogWarning(
                ex,
                "{RequestName} cancelled after {ElapsedSeconds:F2}s",
                messageName,
                stopwatch.Elapsed.TotalSeconds
            );

            throw new RequestTimeoutException(messageName, stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("exception.type", ex.GetType().Name);
            activity?.SetTag("exception.message", ex.Message);
            
            if (!ex.Data.Contains("RequestName"))
                ex.Data["RequestName"] = messageName;
    
            if (!ex.Data.Contains("ElapsedMs"))
                ex.Data["ElapsedMs"] = stopwatch.ElapsedMilliseconds;

            logger.LogError(
                ex,
                "{RequestName} failed after {ElapsedMs}ms",
                messageName,
                stopwatch.ElapsedMilliseconds
            );
            throw;
        }
    }
}