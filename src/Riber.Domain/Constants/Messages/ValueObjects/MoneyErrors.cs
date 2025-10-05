namespace Riber.Domain.Constants.Messages.ValueObjects;

public static class MoneyErrors
{
    public const string NegativeValue = "Valor não pode ser negativo.";
    public const string Sum = "Não é possível somar moedas diferentes.";
    public const string Subtraction = "Não é possível subtrair moedas diferentes.";
    public const string ZeroValue = "O valor deve ser maior que zero";
}