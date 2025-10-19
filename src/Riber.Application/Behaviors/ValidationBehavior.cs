using FluentValidation;
using Mediator;
using LayerValidationException = Riber.Application.Exceptions.ValidationException;

namespace Riber.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IMessage
{
    public async ValueTask<TResponse> Handle(
        TRequest message, 
        MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(message, cancellationToken);
        
        // ValidationContext prepara a request para ficar num formato aceitável pelo FluentValidation.
        var context = new ValidationContext<TRequest>(message);
        var validationErrors = validators
            .Select(x => x.Validate(context))
            .Where(x => !x.IsValid)
            .SelectMany(x => x.Errors)
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );
        
        return validationErrors.Count > 0 
            ? throw new LayerValidationException(validationErrors)
            : await next(message, cancellationToken);
    }
}
