using SnackFlow.Domain.ValueObjects.Email;

namespace SnackFlow.Domain.Abstractions.ValueObjects;

/// <summary>
/// Representa uma entidade que possui um endereço de e-mail associado.
/// </summary>
public interface IHasEmail
{
    /// <summary>
    /// Representa o endereço de e-mail associado a uma entidade.
    /// Esta propriedade encapsula um objeto de valor <see cref="Email"/> que garante a validade
    /// e padronização dos endereços de e-mail.
    /// </summary>
    Email Email { get; }
}