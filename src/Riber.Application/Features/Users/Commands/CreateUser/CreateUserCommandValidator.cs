using FluentValidation;
using Riber.Domain.Constants;
using Riber.Domain.ValueObjects.Email;
using Riber.Domain.ValueObjects.Phone;

namespace Riber.Application.Features.Users.Commands.CreateUser;

internal sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage(ErrorMessage.Name.IsNullOrEmpty);

        RuleFor(x => x.TaxId)
            .NotEmpty()
            .WithMessage(ErrorMessage.Cpf.IsNullOrEmpty)
            .Matches(@"^(?!^(\d)\1{10}$)(\d{3}\.?\d{3}\.?\d{3}-?\d{2})$")
            .WithMessage(ErrorMessage.Cpf.LengthIsInvalid);

        RuleFor(x => x.Position)
            .NotNull()
            .WithMessage("O cargo deve ser preenchido.")
            .IsInEnum()
            .WithMessage("O cargo informado não é válido.");

        When(x => x.CompanyId.HasValue, () =>
        {
            RuleFor(x => x.CompanyId)
                .NotEqual(Guid.Empty)
                .WithMessage(ErrorMessage.Invalid.CompanyId);
        });

        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("O nome de usuário deve ser preenchido.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("A senha deve ser preenchida.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
            .WithMessage("A senha deve ter no mínimo 8 caracteres, incluindo pelo menos: 1 letra minúscula, 1 maiúscula, 1 número e 1 caractere especial (@$!%*?&).");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ErrorMessage.Email.IsNullOrEmpty)
            .Matches(Email.RegexPattern)
            .WithMessage(ErrorMessage.Email.FormatInvalid);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage(ErrorMessage.Phone.IsNullOrEmpty)
            .Matches(Phone.RegexPattern)
            .WithMessage(ErrorMessage.Phone.FormatInvalid);
    }
}