namespace Riber.Domain.Constants.Messages.ValueObjects;

public static class CnpjErrors
{
    public const string Empty = "O CNPJ não pode ser vazio.";
    public const string OnlyRepeatedDigits = "CNPJ não pode estar somente com números iguais.";
    public const string Invalid = "CNPJ está inválido.";
    public const string Length = "CNPJ deve ter 14 dígitos para ser validado.";
}