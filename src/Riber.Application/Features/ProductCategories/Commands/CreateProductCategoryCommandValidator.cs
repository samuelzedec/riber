using FluentValidation;
using Riber.Domain.Constants.Messages.Entities;

namespace Riber.Application.Features.ProductCategories.Commands;

public sealed class CreateProductCategoryCommandValidator : AbstractValidator<CreateProductCategoryCommand>
{
    public CreateProductCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(CategoryErrors.NameEmpty)
            .MaximumLength(255)
            .WithMessage(CategoryErrors.NameLength);

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage(CategoryErrors.CodeEmpty)
            .Length(5)
            .WithMessage(CategoryErrors.CodeLength);

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(CategoryErrors.DescriptionEmpty)
            .MaximumLength(255)
            .WithMessage(CategoryErrors.DescriptionLength);
    }
}