using System.Data;
using FluentValidation;
using Riber.Domain.Constants;

namespace Riber.Application.Features.Product.Commands;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{ 
    public CreateProductCommandValidator() 
    { 
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull()
            .WithMessage(ErrorMessage.Product.NameIsNull)
            .MaximumLength(255)
            .WithMessage("O nome do produto deve ter no máximo 255 caracteres.");
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .NotNull()
            .WithMessage(ErrorMessage.Product.DescriptionIsNull)
            .MaximumLength(255)
            .WithMessage("A descrição do produto deve ter no máximo 255 caracteres.");
        
        RuleFor(x => x.Price)
            .NotEmpty()
            .NotNull()
            .WithMessage("O produto deve ter um preço.")
            .GreaterThan(0)
            .WithMessage("O preço do produto deve ser maior que zero.");
        
        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .NotNull()
            .WithMessage("O produto deve pertencer a uma categoria.");
    }
}