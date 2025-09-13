using Riber.Domain.ValueObjects.FullName;

namespace Riber.Domain.Abstractions.ValueObjects;

/// <summary>
/// Define um contrato para objetos que possuem um nome completo.
/// </summary>
public interface IHasFullName
{
    /// <summary>
    /// Representa o nome completo de uma entidade.
    /// </summary>
    /// <remarks>
    /// O nome completo segue restrições específicas, como um comprimento máximo de 255 caracteres,
    /// um comprimento mínimo de 5 caracteres e um padrão regex para garantir que apenas caracteres válidos sejam usados.
    /// </remarks>
    FullName FullName { get; }
}