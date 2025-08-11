using FluentValidation;

namespace SnackFlow.Application.Features.Auths.Commands.Login;

internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.EmailOrUserName)
            .NotEmpty()
            .WithMessage("O valor informado deve ser um e-mail válido ou um nome de usuário não vazio.");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("A senha não pode ser vazia.");
    }
}