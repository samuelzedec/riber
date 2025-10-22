using Riber.Domain.Abstractions.ValueObjects;
using Riber.Domain.Constants.Messages.ValueObjects;
using Riber.Domain.ValueObjects.Discount;
using Riber.Domain.ValueObjects.Discount.Exceptions;
using Riber.Domain.ValueObjects.Money;
using Riber.Domain.ValueObjects.Quantity;

namespace Riber.Domain.Entities;

public sealed class OrderItem
    : BaseEntity, IHasDiscount, IHasQuantity, IHasUnitPrice
{
    #region Properties

    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public Money UnitPrice { get; private set; }
    public Quantity Quantity { get; private set; }
    public Discount? ItemDiscount { get; private set; }
    public decimal SubTotal => UnitPrice.Multiply(Quantity);
    public decimal DiscountAmount => ItemDiscount?.CalculateDiscount(SubTotal) ?? 0;
    public decimal TotalPrice => SubTotal - DiscountAmount;

    #endregion

    #region Navigation Properties

    public Order Order { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    #endregion

    #region Constructors

#pragma warning disable CS8618, CA1823
    private OrderItem() : base(Guid.Empty) { }
#pragma warning restore CS8618, CA1823

    private OrderItem(
        Guid orderId,
        Guid productId,
        string productName,
        decimal unitPrice,
        int quantity
    ) : base(Guid.CreateVersion7())
    {
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = Money.Create(unitPrice);
        Quantity = Quantity.Create(quantity);
    }

    #endregion

    #region Factories

    public static OrderItem Create(
        Guid orderId,
        Guid productId,
        string productName,
        decimal unitPrice,
        int quantity
    ) => new(orderId, productId, productName, unitPrice, quantity);

    #endregion

    #region Methods

    public void ApplyDiscount(Discount discount)
    {
        if (!discount.HasDiscount())
            throw new InvalidDiscountException(DiscountErrors.FixedAmount);

        var calculatedDiscount = discount.CalculateDiscount(SubTotal);
        if (calculatedDiscount > SubTotal)
            throw new InvalidDiscountException(DiscountErrors.GreaterThanSubtotal);

        ItemDiscount = discount;
    }

    public void RemoveDiscount()
    {
        if (ItemDiscount != null)
            ItemDiscount = null;
    }

    public bool HasDiscount() => ItemDiscount?.HasDiscount() == true;

    #endregion
}