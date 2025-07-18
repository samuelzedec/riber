using FluentValidation;
using Mediator;
using SnackFlow.Application.Common;
using ValidationException = SnackFlow.Application.Exceptions.ValidationException;

namespace SnackFlow.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IMessage
{
    public async ValueTask<TResponse> Handle(
        TRequest request, 
        MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(request, cancellationToken);
        
        // ValidationContext prepara a request para ficar num formato aceitável pelo FluentValidation.
        var context = new ValidationContext<TRequest>(request);
        var validationErrors = validators
            .Select(x => x.Validate(context))
            .Where(x => !x.IsValid)
            .SelectMany(x => x.Errors)
            .Select(x => new ValidationError(x.PropertyName, x.ErrorMessage))
            .ToList();
        
        return validationErrors.Count > 0 
            ? throw new ValidationException(validationErrors)
            : await next(request, cancellationToken);
    }
}
