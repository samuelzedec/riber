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
    public Dictionary<string, string[]>? Details { get; init; }

    #endregion

    #region Constructors

    [JsonConstructor]
    public Error() { }

    public Error(string message, HttpStatusCode statusCode, Dictionary<string, string[]>? details)
    {
        Message = message;
        Type = statusCode.ToString();
        Details = details;
    }

    #endregion
}