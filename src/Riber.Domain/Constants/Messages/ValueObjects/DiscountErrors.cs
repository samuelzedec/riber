namespace Riber.Domain.Constants.Messages.ValueObjects;

public static class DiscountErrors
{
    public const string Percentage = "O valor do desconto deve ser maior que zero ou menor ou igual a 100%.";
    public const string FixedAmount = "O valor do desconto deve ser maior que zero.";
    public const string GreaterThanSubtotal = "Desconto não pode ser maior que o subtotal.";
}