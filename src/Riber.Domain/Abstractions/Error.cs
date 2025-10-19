using System.Text.Json.Serialization;

namespace Riber.Domain.Abstractions;

/// <summary>
/// Representa um erro com um código identificador e uma mensagem descritiva.
/// </summary>
public sealed class Error
{
    #region Properties

    public string Type { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public Dictionary<string, string[]> Details { get; set; } = [];

    #endregion

    #region Constructors

    [JsonConstructor]
    public Error() { }

    public Error(string type, string message)
    {
        Type = type;
        Message = message;
    }

    public Error(string type, Dictionary<string, string[]> details)
    {
        Type = type;
        Message = "Dados Inválidos.";
        Details = details;
    }

    #endregion
}