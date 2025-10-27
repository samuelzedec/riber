using System.Net;
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

    public Error(string message, HttpStatusCode statusCode)
    {
        Message = message;
        Type = statusCode.ToString();
    }

    public Error(Dictionary<string, string[]> details, HttpStatusCode statusCode)
    {
        Message = "Dados Inválidos.";
        Details = details;
        Type = statusCode.ToString();
    }

    #endregion
}