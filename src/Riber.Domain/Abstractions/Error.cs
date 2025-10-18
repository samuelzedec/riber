namespace Riber.Domain.Abstractions;

/// <summary>
/// Representa um erro com um código identificador e uma mensagem descritiva.
/// </summary>
/// <param name="Code">Código único que identifica o tipo de erro.</param>
/// <param name="Messages">Mensagens descritivas e legível sobre os errors.</param>
public record Error(string Code, params string[] Messages)
{
    public static readonly Error None = new(string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "Um valor nulo foi fornecido!");
}