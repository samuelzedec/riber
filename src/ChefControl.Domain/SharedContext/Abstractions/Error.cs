namespace ChefControl.Domain.SharedContext.Abstractions;

public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, String.Empty);
    public static Error NullValue = new("Error.NullValue", "Um valor nulo foi fornecido!");
}