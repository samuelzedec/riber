namespace Riber.Domain.Constants.Messages.Entities;

public static class ProductErrors
{
    public const string NameEmpty = "O nome do produto não pode estar vazio.";
    public const string NameLength = "O nome do produto deve ter no máximo 255 caracteres.";
    public const string DescriptionEmpty = "A descrição do produto não pode estar vazia.";
    public const string DescriptionLength = "A descrição do produto deve ter no máximo 255 caracteres.";
    public const string PriceEmpty = "O produto deve ter um preço.";
    public const string PriceGreaterThanZero = "O preço do produto deve ser maior que zero.";
    public const string InvalidCategory = "O produto deve pertencer a uma categoria.";
}