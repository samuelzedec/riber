using System.Text.Json.Serialization;

namespace Riber.Domain.Abstractions;

/// <summary>
/// Representa um erro com um código identificador e uma mensagem descritiva.
/// </summary>
public sealed class Error
{
    #region Properties

    public string Code { get; init; } = string.Empty;
    public string[] Messages { get; init; } = [];

    #endregion

    #region Constructors

    [JsonConstructor]
    public Error() {}
    
    public Error(string code, params string[] messages)
    {
        Code = code;
        Messages = messages;
    }
    
    #endregion
}