using System.Diagnostics;
using Mediator;
using Microsoft.Extensions.Logging;
using SnackFlow.Application.Exceptions;

namespace SnackFlow.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IMessage
{
    public async ValueTask<TResponse> Handle(
        TRequest request, 
        MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestFullName = request.GetType().FullName ?? request.GetType().Name;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            logger.LogInformation("Handling request: {RequestFullName}", requestFullName);
            var result = await next(request, cancellationToken);

            logger.LogInformation("Request {RequestFullName} processed in {ElapsedMs}ms.",
                requestFullName, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            var elapsedTime = stopwatch.Elapsed;
            logger.LogWarning("Request {RequestFullName} timed out after {ElapsedSeconds}s",
                requestFullName, elapsedTime.TotalSeconds);

            throw new RequestTimeoutException(requestFullName, elapsedTime);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while handling request: {RequestFullName} after {ElapsedMs}ms",
                requestFullName, stopwatch.ElapsedMilliseconds);
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }
    }
}