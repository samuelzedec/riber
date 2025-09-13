using Riber.Domain.ValueObjects.Phone;

namespace Riber.Domain.Abstractions.ValueObjects;

/// <summary>
/// Representa uma abstração para um tipo que inclui detalhes de telefone.
/// Serve como uma interface marcadora para objetos que possuem informações relacionadas a telefone.
/// </summary>
public interface IHasPhone
{
    /// <summary>
    /// Representa um objeto de valor de número de telefone.
    /// Encapsula as propriedades e comportamentos associados a um número de telefone,
    /// incluindo validação, formatação e transformação.
    /// Este é um tipo imutável, garantindo a integridade dos dados do número de telefone.
    /// </summary>
    Phone Phone { get; }
}