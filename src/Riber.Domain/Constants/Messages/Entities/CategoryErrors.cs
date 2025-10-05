namespace Riber.Domain.Constants.Messages.Entities;

public static class CategoryErrors
{
    public const string NameEmpty = "O nome da categoria não pode estar vazio.";
    public const string NameLength = "O nome da categoria deve ter no máximo 255 caracteres.";
    public const string CodeEmpty = "O código da categoria não pode estar vazio.";
    public const string CodeLength = "O código da categoria deve ter 5 caracteres.";
    public const string DescriptionEmpty = "A descrição da categoria não pode estar vazia.";
    public const string DescriptionLength = "A descrição da categoria deve ter no máximo 255 caracteres.";
    public const string Invalid = "A categoria selecionada não é válida.";
}