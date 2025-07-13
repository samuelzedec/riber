using FluentValidation;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.ValueObjects.CompanyName;
using SnackFlow.Domain.ValueObjects.Email;
using SnackFlow.Domain.ValueObjects.Phone;

namespace SnackFlow.Application.Features.Companies.Commands.UpdateCompany;

public sealed class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyCommandValidator()
    {
        RuleFor(x => x.TradingName)
            .Length(CompanyName.MinLength, CompanyName.FantasyMaxLength)
            .WithMessage(ErrorMessage.Name.LengthIsInvalid(CompanyName.MinLength, CompanyName.FantasyMaxLength))
            .When(x => !string.IsNullOrEmpty(x.TradingName));

        RuleFor(x => x.Email)
            .Matches(Email.EmailRegexPattern)
            .WithMessage(ErrorMessage.Email.FormatInvalid)
            .When(x => !string.IsNullOrEmpty(x.Email));
        
        RuleFor(x => x.Phone)
            .Matches(Phone.PhoneRegexPattern)
            .WithMessage(ErrorMessage.Phone.FormatInvalid)
            .When(x => !string.IsNullOrEmpty(x.Phone));
    }
}