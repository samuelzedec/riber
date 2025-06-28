using FluentValidation;
using MediatR;
using SnackFlow.Application.Common;
using ValidationException = SnackFlow.Application.Exceptions.ValidationException;

namespace SnackFlow.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);
        
        // ValidationContext prepara a request para ficar num formato aceitável pelo FluentValidation.
        var context = new ValidationContext<TRequest>(request);
        var validationErrors = validators
            .Select(x => x.Validate(context))
            .Where(x => x.Errors.Count > 0)
            .SelectMany(x => x.Errors)
            .Select(x => new ValidationError(x.PropertyName, x.ErrorMessage))
            .ToList();
        
        return validationErrors.Count > 0 
            ? throw new ValidationException(validationErrors)
            : await next(cancellationToken);
    }
}
