using SnackFlow.Domain.ValueObjects.Discount;

namespace SnackFlow.Domain.Abstractions.ValueObjects;

/// <summary>
/// Define um contrato para objetos que possuem um desconto associado.
/// </summary>
public interface IHasDiscount
{
    /// <summary>
    /// Representa o desconto aplicado a um item, encapsulando informações sobre
    /// descontos baseados em porcentagem ou valor fixo junto com um motivo opcional.
    /// </summary>
    /// <remarks>
    /// Esta propriedade fornece acesso a uma instância de <see cref="SnackFlow.Domain.ValueObjects.Discount.Discount"/>,
    /// que contém detalhes sobre o desconto aplicável ao respectivo item.
    /// O desconto pode incluir valores baseados em porcentagem ou valor fixo, e fornece métodos utilitários
    /// para calcular descontos efetivos ou verificar o tipo de desconto sendo aplicado.
    /// </remarks>
    Discount? ItemDiscount { get; }
}