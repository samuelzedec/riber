using FluentValidation;
using Riber.Domain.Constants;
using Riber.Domain.Enums;
using Riber.Domain.ValueObjects.CompanyName;
using Riber.Domain.ValueObjects.Email;
using Riber.Domain.ValueObjects.Phone;

namespace Riber.Application.Features.Companies.Commands.CreateCompanyWithAdmin;

internal sealed class CreateCompanyWithAdminCommandValidator : AbstractValidator<CreateCompanyWithAdminCommand>
{
    public CreateCompanyWithAdminCommandValidator()
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
        
        RuleFor(x => x.AdminFullName)
            .NotEmpty()
            .WithMessage(ErrorMessage.Name.IsNullOrEmpty);

        RuleFor(x => x.TaxId)
            .NotEmpty()
            .WithMessage(ErrorMessage.Cpf.IsNullOrEmpty)
            .Matches(@"^(?!^(\d)\1{10}$)(\d{3}\.?\d{3}\.?\d{3}-?\d{2})$")
            .WithMessage(ErrorMessage.Cpf.LengthIsInvalid);
        
        RuleFor(x => x.AdminUserName)
            .NotEmpty()
            .WithMessage("O nome de usuário deve ser preenchido.");
        
        RuleFor(x => x.AdminPassword)
            .NotEmpty()
            .WithMessage("A senha deve ser preenchida.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
            .WithMessage("A senha deve ter no mínimo 8 caracteres, incluindo pelo menos: 1 letra minúscula, 1 maiúscula, 1 número e 1 caractere especial (@$!%*?&).");
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ErrorMessage.Email.IsNullOrEmpty)
            .Matches(Email.RegexPattern)
            .WithMessage(ErrorMessage.Email.FormatInvalid);
        
        RuleFor(x => x.AdminPhoneNumber)
            .NotEmpty()
            .WithMessage(ErrorMessage.Phone.IsNullOrEmpty)
            .Matches(Phone.RegexPattern)
            .WithMessage(ErrorMessage.Phone.FormatInvalid);
    }
}