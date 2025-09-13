using FluentValidation;
using Riber.Domain.Constants;
using Riber.Domain.ValueObjects.CompanyName;
using Riber.Domain.ValueObjects.Email;
using Riber.Domain.ValueObjects.Phone;

namespace Riber.Application.Features.Companies.Commands.UpdateCompany;

internal sealed class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyCommandValidator()
    {
        RuleFor(x => x.FantasyName)
            .Length(CompanyName.MinLength, CompanyName.FantasyMaxLength)
            .WithMessage(ErrorMessage.Name.LengthIsInvalid(CompanyName.MinLength, CompanyName.FantasyMaxLength))
            .When(x => !string.IsNullOrEmpty(x.FantasyName));

        RuleFor(x => x.Email)
            .Matches(Email.RegexPattern)
            .WithMessage(ErrorMessage.Email.FormatInvalid)
            .When(x => !string.IsNullOrEmpty(x.Email));
        
        RuleFor(x => x.Phone)
            .Matches(Phone.RegexPattern)
            .WithMessage(ErrorMessage.Phone.FormatInvalid)
            .When(x => !string.IsNullOrEmpty(x.Phone));
    }
}