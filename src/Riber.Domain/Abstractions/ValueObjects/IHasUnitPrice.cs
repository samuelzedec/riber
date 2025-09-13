using Riber.Domain.ValueObjects.Money;

namespace Riber.Domain.Abstractions.ValueObjects;

/// <summary>
/// Representa um contrato para objetos que possuem preço unitário.
/// Classes que implementam esta interface devem fornecer acesso a um valor 
/// monetário que representa o preço por unidade do item.
/// </summary>
public interface IHasUnitPrice
{
    /// <summary>
    /// Preço unitário do item representado como objeto de valor monetário.
    /// Encapsula valor e moeda, garantindo operações monetárias válidas
    /// e prevenindo valores negativos.
    /// </summary>
    Money UnitPrice { get; }
}