using ChefControl.Application.SharedContext.Abstractions;
using FluentValidation;
using MediatR;
using AppValidationException = ChefControl.Application.SharedContext.Exceptions.ValidationException;

namespace ChefControl.Application.SharedContext.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);
        
        // ValidationContext prepara a request para que fique em um formato aceitável pelo FluentValidation.
        var context = new ValidationContext<TRequest>(request);
        var validationErrors = validators
            .Select(x => x.Validate(context))
            .Where(x => x.Errors.Count > 0)
            .SelectMany(x => x.Errors)
            .Select(x => new ValidationError(x.PropertyName, x.ErrorMessage))
            .ToList();
        
        return validationErrors.Count > 0 
            ? throw new AppValidationException(validationErrors)
            : await next(cancellationToken);
    }
}
