using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using SnackFlow.Application.SharedContext.Exceptions;

namespace SnackFlow.Application.SharedContext.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestFullName = request.GetType().FullName ?? request.GetType().Name;
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            logger.LogInformation("Handling request: {RequestFullName} in {ElapsedMs}ms.", 
                requestFullName, stopwatch.ElapsedMilliseconds);
            
            var result = await next(cancellationToken);
            
            logger.LogInformation("Request {RequestFullName} processed in {ElapsedMs}ms.", 
                requestFullName, stopwatch.ElapsedMilliseconds);
            
            return result;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            var elapsedTime = stopwatch.Elapsed;
            
            logger.LogWarning("Request {RequestName} timed out after {ElapsedSeconds}s", 
                requestFullName, elapsedTime.TotalSeconds);
            
            throw new RequestTimeoutException(requestFullName, elapsedTime);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while handling request: {RequestName} after {ElapsedMs}ms", 
                requestFullName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}