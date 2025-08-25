using SnackFlow.Domain.ValueObjects.Quantity;

namespace SnackFlow.Domain.Abstractions.ValueObjects;

/// <summary>
/// Representa uma abstração para objetos que possuem uma quantidade associada a eles.
/// </summary>
/// <remarks>
/// A interface <see cref="IHasQuantity"/> define um contrato para recuperar
/// uma quantidade como um objeto de valor. Isso pode ser implementado por entidades de domínio ou objetos de valor
/// que encapsulam ou requerem uma propriedade de quantidade.
/// </remarks>
public interface IHasQuantity
{
    /// <summary>
    /// Representa o objeto de valor para quantidade, encapsulando seu valor e comportamentos relacionados.
    /// </summary>
    /// <remarks>
    /// Esta classe garante que a quantidade é sempre um número inteiro positivo, fornecendo validação
    /// durante a criação e aplicando regras específicas do domínio. É imutável, e qualquer operação
    /// que modifique seu valor retorna uma nova instância.
    /// </remarks>
    Quantity Quantity { get; }
}