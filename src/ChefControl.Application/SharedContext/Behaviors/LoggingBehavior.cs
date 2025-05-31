using MediatR;
using Microsoft.Extensions.Logging;

namespace ChefControl.Application.SharedContext.Behaviors;

public class LoggingBehavior<TRequest, TReponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TReponse> where TRequest : IRequest
{
    public async Task<TReponse> Handle(TRequest request, RequestHandlerDelegate<TReponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Handling request: {FullName}.", request.GetType().FullName);
            var result = await next(cancellationToken);
            logger.LogInformation("Request {FullName} processed.", request.GetType().FullName);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while handling request: {FullName}", request.GetType().FullName);
            throw;
        }
    }
}