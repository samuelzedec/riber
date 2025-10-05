namespace Riber.Domain.Constants.Messages.Common;

public static class PasswordErrors
{
    public const string Empty = "A senha não deve estar vazia.";
    public const string Invalid = "A senha é inválida.";
    public const string Format =
        "A senha deve ter no mínimo 8 caracteres, incluindo pelo menos: 1 letra minúscula, 1 maiúscula, 1 número e 1 caractere especial (@$!%*?&).";
}