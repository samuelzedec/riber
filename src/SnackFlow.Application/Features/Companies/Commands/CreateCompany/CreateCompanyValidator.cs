using FluentValidation;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Enums;
using SnackFlow.Domain.ValueObjects.Email;
using SnackFlow.Domain.ValueObjects.Phone;
using SnackFlow.Domain.ValueObjects.CompanyName;

namespace SnackFlow.Application.Features.Companies.Commands.CreateCompany;

public class CreateCompanyValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ErrorMessage.Name.IsNullOrEmpty)
            .Length(CompanyName.MinLength, CompanyName.NameMaxLength)
            .WithMessage(ErrorMessage.Name.LengthIsInvalid(CompanyName.MinLength, CompanyName.NameMaxLength));
        
        RuleFor(x => x.TradingName)
            .NotEmpty()
            .WithMessage(ErrorMessage.TradingName.IsNullOrEmpty)
            .Length(CompanyName.MinLength, CompanyName.TradingNameMaxLength)
            .WithMessage(ErrorMessage.Name.LengthIsInvalid(CompanyName.MinLength, CompanyName.TradingNameMaxLength));

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ErrorMessage.Email.IsNullOrEmpty)
            .Matches(Email.EmailRegexPattern)
            .WithMessage(ErrorMessage.Email.FormatInvalid);
        
        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage(ErrorMessage.Phone.IsNullOrEmpty)
            .Matches(Phone.PhoneRegexPattern)
            .WithMessage(ErrorMessage.Phone.FormatInvalid);

        RuleFor(x => x.Type)
            .NotNull()
            .WithMessage(ErrorMessage.Document.IsNullOrEmpty)
            .IsInEnum()
            .WithMessage(ErrorMessage.Document.IsInvalid);

        RuleFor(x => x.TaxId)
            .NotEmpty()
            .WithMessage(ErrorMessage.Cpf.IsNullOrEmpty)
            .Matches(@"^\d{11}$")
            .WithMessage(ErrorMessage.Cpf.LengthIsInvalid)
            .When(x => x.Type == ECompanyType.IndividualWithCpf);
        
        RuleFor(x => x.TaxId)
            .NotEmpty()
            .WithMessage(ErrorMessage.Cnpj.IsNullOrEmpty)
            .Matches(@"^\d{14}$")
            .WithMessage(ErrorMessage.Cnpj.LengthIsInvalid)
            .When(x => x.Type == ECompanyType.LegalEntityWithCnpj);
    }
}