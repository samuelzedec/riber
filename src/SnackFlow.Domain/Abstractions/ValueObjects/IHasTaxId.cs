using SnackFlow.Domain.ValueObjects.TaxId;

namespace SnackFlow.Domain.Abstractions.ValueObjects;

/// <summary>
/// Representa uma interface que define um contrato para objetos que contém um objeto de valor CPF/CNPJ.
/// </summary>
public interface IHasTaxId
{
    /// <summary>
    /// Representa um documento de identificação fiscal (TaxId), que pode ser um
    /// CPF (Cadastro de Pessoas Físicas) para pessoas físicas ou
    /// CNPJ (Cadastro Nacional da Pessoa Jurídica) para pessoas jurídicas no contexto brasileiro.
    /// </summary>
    /// <remarks>
    /// O TaxId é representado como um objeto de valor e pode ser de dois tipos:
    /// - CPF: Número de identificação fiscal individual
    /// - CNPJ: Número de identificação fiscal de pessoa jurídica
    /// </remarks>
    TaxId TaxId { get; }
}