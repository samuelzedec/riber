namespace ChefControl.Domain.SharedContext.Abstractions;

/// <summary>
/// Representa um erro com um código identificador e uma mensagem descritiva.
/// </summary>
/// <param name="Code">Código único que identifica o tipo de erro.</param>
/// <param name="Message">Mensagem descritiva e legível para humanos sobre o erro.</param>
public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, String.Empty);
    public static Error NullValue = new("Error.NullValue", "Um valor nulo foi fornecido!");
}