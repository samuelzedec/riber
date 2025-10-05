namespace Riber.Domain.Constants.Messages.ValueObjects;

public static class CpfErrors
{
    public const string Empty = "O CPF não pode estar vazio.";
    public const string OnlyRepeatedDigits = "CPF não pode estar somente com números iguais.";
    public const string Invalid = "CPF está inválido.";
    public const string Length = "CPF deve ter 11 dígitos para ser validado.";
}