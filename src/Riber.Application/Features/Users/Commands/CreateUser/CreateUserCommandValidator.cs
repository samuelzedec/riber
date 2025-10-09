using FluentValidation;
using Riber.Domain.Constants.Messages.Entities;
using Riber.Domain.Constants.Messages.ValueObjects;
using Riber.Domain.ValueObjects.Email;
using Riber.Domain.ValueObjects.Phone;

namespace Riber.Application.Features.Users.Commands.CreateUser;

internal sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage(NameErrors.FullNameEmpty);

        RuleFor(x => x.TaxId)
            .NotEmpty()
            .WithMessage(CpfErrors.Empty)
            .Matches(@"^(?!^(\d)\1{10}$)(\d{3}\.?\d{3}\.?\d{3}-?\d{2})$")
            .WithMessage(CpfErrors.Invalid);

        RuleFor(x => x.Position)
            .NotNull()
            .WithMessage("O cargo deve ser preenchido.")
            .IsInEnum()
            .WithMessage("O cargo informado não é válido.");

        When(x => x.CompanyId.HasValue, () => RuleFor(x => x.CompanyId)
            .NotEqual(Guid.Empty)
            .WithMessage(CompanyErrors.Invalid));

        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("O nome de usuário deve ser preenchido.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("A senha deve ser preenchida.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
            .WithMessage(
                "A senha deve ter no mínimo 8 caracteres, incluindo pelo menos: 1 letra minúscula, 1 maiúscula, 1 número e 1 caractere especial (@$!%*?&).");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(EmailErrors.Empty)
            .Matches(Email.RegexPattern)
            .WithMessage(EmailErrors.Format);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage(PhoneErrors.Empty)
            .Matches(Phone.RegexPattern)
            .WithMessage(PhoneErrors.Format);
    }
}