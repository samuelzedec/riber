using FluentValidation;
using Riber.Domain.Constants.Messages.Entities;

namespace Riber.Application.Features.Products.Commands;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull()
            .WithMessage(ProductErrors.NameEmpty)
            .MaximumLength(255)
            .WithMessage(ProductErrors.NameLength);

        RuleFor(x => x.Description)
            .NotEmpty()
            .NotNull()
            .WithMessage(ProductErrors.DescriptionEmpty)
            .MaximumLength(255)
            .WithMessage(ProductErrors.DescriptionLength);

        RuleFor(x => x.Price)
            .NotEmpty()
            .NotNull()
            .WithMessage(ProductErrors.PriceEmpty)
            .GreaterThan(0)
            .WithMessage(ProductErrors.PriceGreaterThanZero);

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .NotNull()
            .WithMessage(ProductErrors.InvalidCategory);
    }
}