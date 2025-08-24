using SnackFlow.Domain.ValueObjects.CompanyName;

namespace SnackFlow.Domain.Abstractions.ValueObjects;

/// <summary>
/// Define uma abstração para objetos que têm um nome de empresa.
/// </summary>
/// <remarks>
/// Esta interface é usada para representar entidades ou objetos de valor dentro do domínio
/// que incluem ou requerem um nome de empresa como parte de sua definição.
/// O nome da empresa é encapsulado usando o objeto de valor <c>CompanyName</c>,
/// que impõe regras específicas do domínio para a validade dos nomes de empresa.
/// </remarks>
public interface IHasCompanyName
{
    /// <summary>
    /// Obtém o nome da empresa associado ao objeto.
    /// </summary>
    /// <remarks>
    /// A propriedade representa o nome da empresa, que é encapsulado no
    /// objeto de valor <c>CompanyName</c>. Esta abstração garante que o nome
    /// da empresa siga as regras de negócio definidas no domínio, como comprimento e validade.
    /// </remarks>
    CompanyName Name { get; }
}