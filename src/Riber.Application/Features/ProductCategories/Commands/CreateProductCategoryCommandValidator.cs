using FluentValidation;
using Riber.Domain.Constants;

namespace Riber.Application.Features.ProductCategories.Commands;

public sealed class CreateProductCategoryCommandValidator : AbstractValidator<CreateProductCategoryCommand>
{ 
    public CreateProductCategoryCommandValidator() 
    { 
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull()
            .WithMessage(ErrorMessage.Product.CategoryNameIsNull)
            .MaximumLength(100)
            .WithMessage("O nome da categoria deve ter no máximo 100 caracteres.");
        
        RuleFor(x => x.Code)
            .NotEmpty()
            .NotNull()
            .WithMessage(ErrorMessage.Product.CategoryCodeIsNull)
            .Length(5)
            .WithMessage("O código da categoria deve ter 5 caracteres.");
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .NotNull()
            .WithMessage("A descrição da categoria não pode ser nula.")
            .MaximumLength(255)
            .WithMessage("A descrição da categoria deve ter no máximo 255 caracteres.");
    }
}