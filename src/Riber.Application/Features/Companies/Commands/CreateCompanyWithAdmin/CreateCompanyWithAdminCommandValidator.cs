using FluentValidation;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Constants.Messages.ValueObjects;
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
            .WithMessage(NameErrors.CorporateNameEmpty)
            .Length(CompanyName.MinLength, CompanyName.CorporateMaxLength)
            .WithMessage(NameErrors.CorporateNameLength(CompanyName.MinLength, CompanyName.CorporateMaxLength));

        RuleFor(x => x.FantasyName)
            .NotEmpty()
            .WithMessage(NameErrors.FantasyNameEmpty)
            .Length(CompanyName.MinLength, CompanyName.FantasyMaxLength)
            .WithMessage(NameErrors.FantasyNameLength(CompanyName.MinLength, CompanyName.FantasyMaxLength));

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(EmailErrors.Empty)
            .Matches(Email.RegexPattern)
            .WithMessage(EmailErrors.Format);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage(PhoneErrors.Empty)
            .Matches(Phone.RegexPattern)
            .WithMessage(PhoneErrors.Format);

        When(x => x.Type == TaxIdType.IndividualWithCpf, () =>
        {
            RuleFor(x => x.TaxId)
                .NotEmpty()
                .WithMessage(CpfErrors.Empty);
                
            RuleFor(x => x.TaxId)
                .Matches(@"^(?!^(\d)\1{10}$)(\d{3}\.?\d{3}\.?\d{3}-?\d{2})$")
                .WithMessage(CpfErrors.Length);
        });
        
        When(x => x.Type == TaxIdType.LegalEntityWithCnpj, () =>
        {
            RuleFor(x => x.TaxId)
                .NotEmpty()
                .WithMessage(CnpjErrors.Empty);
                
            RuleFor(x => x.TaxId)
                .Matches(@"^(?!^(\d)\1{13}$)(\d{2}\.?\d{3}\.?\d{3}/?\d{4}-?\d{2})$")
                .WithMessage(CnpjErrors.Length);
        });

        RuleFor(x => x.AdminFullName)
            .NotEmpty()
            .WithMessage(NameErrors.FantasyNameEmpty);

        RuleFor(x => x.AdminTaxId)
            .NotEmpty()
            .WithMessage(CpfErrors.Empty)
            .Matches(@"^(?!^(\d)\1{10}$)(\d{3}\.?\d{3}\.?\d{3}-?\d{2})$")
            .WithMessage(CpfErrors.Invalid);

        RuleFor(x => x.AdminUserName)
            .NotEmpty()
            .WithMessage(NameErrors.UserNameEmpty);

        RuleFor(x => x.AdminPassword)
            .NotEmpty()
            .WithMessage(PasswordErrors.Empty)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
            .WithMessage(PasswordErrors.Format);

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(EmailErrors.Empty)
            .Matches(Email.RegexPattern)
            .WithMessage(EmailErrors.Format);

        RuleFor(x => x.AdminPhoneNumber)
            .NotEmpty()
            .WithMessage(PhoneErrors.Empty)
            .Matches(Phone.RegexPattern)
            .WithMessage(PhoneErrors.Format);
    }
}