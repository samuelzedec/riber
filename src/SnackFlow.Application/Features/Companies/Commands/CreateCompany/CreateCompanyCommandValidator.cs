using FluentValidation;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Enums;
using SnackFlow.Domain.ValueObjects.Email;
using SnackFlow.Domain.ValueObjects.Phone;
using SnackFlow.Domain.ValueObjects.CompanyName;

namespace SnackFlow.Application.Features.Companies.Commands.CreateCompany;

internal sealed class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.CorporateName)
            .NotEmpty()
            .WithMessage(ErrorMessage.Name.IsNullOrEmpty)
            .Length(CompanyName.MinLength, CompanyName.CorporateMaxLength)
            .WithMessage(ErrorMessage.Name.LengthIsInvalid(CompanyName.MinLength, CompanyName.CorporateMaxLength));
        
        RuleFor(x => x.FantasyName)
            .NotEmpty()
            .WithMessage(ErrorMessage.FantasyName.IsNullOrEmpty)
            .Length(CompanyName.MinLength, CompanyName.FantasyMaxLength)
            .WithMessage(ErrorMessage.Name.LengthIsInvalid(CompanyName.MinLength, CompanyName.FantasyMaxLength));

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ErrorMessage.Email.IsNullOrEmpty)
            .Matches(Email.RegexPattern)
            .WithMessage(ErrorMessage.Email.FormatInvalid);
        
        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage(ErrorMessage.Phone.IsNullOrEmpty)
            .Matches(Phone.RegexPattern)
            .WithMessage(ErrorMessage.Phone.FormatInvalid);

        RuleFor(x => x.Type)
            .NotNull()
            .WithMessage(ErrorMessage.Document.IsNullOrEmpty)
            .IsInEnum()
            .WithMessage(ErrorMessage.Document.IsInvalid);

        RuleFor(x => x.TaxId)
            .NotEmpty()
            .WithMessage(ErrorMessage.Cpf.IsNullOrEmpty)
            .Matches(@"^(?!^(\d)\1{10}$)(\d{3}\.?\d{3}\.?\d{3}-?\d{2})$")
            .WithMessage(ErrorMessage.Cpf.LengthIsInvalid)
            .When(x => x.Type == TaxIdType.IndividualWithCpf);
        
        RuleFor(x => x.TaxId)
            .NotEmpty()
            .WithMessage(ErrorMessage.Cnpj.IsNullOrEmpty)
            .Matches(@"^(?!^(\d)\1{13}$)(\d{2}\.?\d{3}\.?\d{3}/?\d{4}-?\d{2})$")
            .WithMessage(ErrorMessage.Cnpj.LengthIsInvalid)
            .When(x => x.Type == TaxIdType.LegalEntityWithCnpj);
    }
}