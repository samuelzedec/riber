using FluentAssertions;
using Riber.Domain.Abstractions.ValueObjects;
using Riber.Domain.Constants;
using Riber.Domain.Entities;
using Riber.Domain.ValueObjects.Discount;
using Riber.Domain.ValueObjects.Discount.Exceptions;

namespace Riber.Domain.Tests.Entities;

public sealed class OrderItemTests : BaseTest
{
    #region Valid Tests

    [Fact(DisplayName = "Should create order item with valid parameters")]
    public void Create_WhenValidParameters_ShouldCreateOrderItem()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var productName = _faker.Commerce.ProductName();
        var unitPrice = _faker.Random.Decimal(1, 1000);
        var quantity = _faker.Random.Int(1, 100);

        // Act
        var result = OrderItem.Create(orderId, productId, productName, unitPrice, quantity);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.OrderId.Should().Be(orderId);
        result.ProductId.Should().Be(productId);
        result.ProductName.Should().Be(productName);
        result.UnitPrice.Value.Should().Be(unitPrice);
        result.Quantity.Value.Should().Be(quantity);
        result.ItemDiscount.Should().BeNull();
        result.HasDiscount().Should().BeFalse();
    }

    [Theory(DisplayName = "Should calculate subtotal correctly")]
    [InlineData(10.50, 2, 21.00)]
    [InlineData(25, 4, 100)]
    [InlineData(33.33, 3, 99.99)]
    [InlineData(100, 1, 100)]
    public void SubTotal_WhenCalculated_ShouldReturnCorrectValue(decimal unitPrice, int quantity, decimal expected)
    {
        // Arrange
        var orderItem = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Test Product", unitPrice, quantity);

        // Act
        var result = orderItem.SubTotal;

        // Assert
        result.Should().Be(expected);
    }

    [Fact(DisplayName = "Should calculate total price correctly without discount")]
    public void TotalPrice_WhenNoDiscount_ShouldEqualSubTotal()
    {
        // Arrange
        var unitPrice = _faker.Random.Decimal(1, 100);
        var quantity = _faker.Random.Int(1, 10);
        var orderItem = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Test Product", unitPrice, quantity);

        // Act
        var subTotal = orderItem.SubTotal;
        var totalPrice = orderItem.TotalPrice;

        // Assert
        totalPrice.Should().Be(subTotal);
        orderItem.DiscountAmount.Should().Be(0);
    }

    [Fact(DisplayName = "Should limit fixed discount to subtotal making product free")]
    public void ApplyDiscount_WhenFixedDiscountGreaterThanSubTotal_ShouldLimitDiscountAndMakeProductFree()
    {
        // Arrange
        var orderItem = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Test Product", 10, 1); // SubTotal = R$ 10
        var discount = Discount.CreateByFixedAmount(20, "Desconto maior");

        // Act
        orderItem.ApplyDiscount(discount);

        // Assert
        orderItem.ItemDiscount.Should().NotBeNull();
        orderItem.DiscountAmount.Should().Be(10);
        orderItem.TotalPrice.Should().Be(0);
        orderItem.HasDiscount().Should().BeTrue();
    }
    
    #endregion

    #region Invalid Tests

    [Theory(DisplayName = "Should throw exception when creating with invalid unit price")]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(-0.01)]
    public void Create_WhenInvalidUnitPrice_ShouldThrowException(decimal invalidUnitPrice)
    {
        // Act
        var act = () => OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Test Product", invalidUnitPrice, 1);

        // Assert
        act.Should().Throw<Exception>(); // Ajuste conforme sua exception específica
    }

    [Theory(DisplayName = "Should throw exception when creating with invalid quantity")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Create_WhenInvalidQuantity_ShouldThrowException(int invalidQuantity)
    {
        // Act
        var act = () => OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Test Product", 10, invalidQuantity);

        // Assert
        act.Should().Throw<Exception>(); // Ajuste conforme sua exception específica
    }

    #endregion

    #region Discount Tests

    [Theory(DisplayName = "Should apply percentage discount correctly")]
    [InlineData(100, 10, 10)] // 10% de R$ 100 = R$ 10 desconto
    [InlineData(50, 20, 10)]  // 20% de R$ 50 = R$ 10 desconto
    [InlineData(25, 50, 12.5)] // 50% de R$ 25 = R$ 12,5 desconto
    public void ApplyDiscount_WhenPercentageDiscount_ShouldCalculateCorrectly(decimal subTotal, decimal percentage, decimal expectedDiscount)
    {
        // Arrange
        var orderItem = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Test Product", subTotal, 1);
        var discount = Discount.CreateByPercentage(percentage, "Test discount");

        // Act
        orderItem.ApplyDiscount(discount);

        // Assert
        orderItem.ItemDiscount.Should().NotBeNull();
        orderItem.HasDiscount().Should().BeTrue();
        orderItem.DiscountAmount.Should().Be(expectedDiscount);
        orderItem.TotalPrice.Should().Be(subTotal - expectedDiscount);
    }

    [Theory(DisplayName = "Should apply fixed amount discount correctly")]
    [InlineData(100, 20, 20)]  // Desconto R$ 20 em R$ 100
    [InlineData(50, 30, 30)]   // Desconto R$ 30 em R$ 50
    [InlineData(25, 50, 25)]   // Desconto R$ 50 em R$ 25 (limitado ao subtotal)
    public void ApplyDiscount_WhenFixedAmountDiscount_ShouldCalculateCorrectly(decimal subTotal, decimal discountAmount, decimal expectedDiscount)
    {
        // Arrange
        var orderItem = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Test Product", subTotal, 1);
        var discount = Discount.CreateByFixedAmount(discountAmount, "Test discount");

        // Act
        orderItem.ApplyDiscount(discount);

        // Assert
        orderItem.ItemDiscount.Should().NotBeNull();
        orderItem.HasDiscount().Should().BeTrue();
        orderItem.DiscountAmount.Should().Be(expectedDiscount);
        orderItem.TotalPrice.Should().Be(subTotal - expectedDiscount);
    }

    [Fact(DisplayName = "Should throw exception when applying invalid discount")]
    public void ApplyDiscount_WhenInvalidDiscount_ShouldThrowInvalidDiscountException()
    {
        // Arrange
        var orderItem = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Test Product", 100, 1);
        var invalidDiscount = Discount.None();

        // Act
        var act = () => orderItem.ApplyDiscount(invalidDiscount);

        // Assert
        act.Should().Throw<InvalidDiscountException>()
           .WithMessage(ErrorMessage.Discount.LessThanOrEqualToZero);
    }

    [Fact(DisplayName = "Should remove discount correctly")]
    public void RemoveDiscount_WhenDiscountExists_ShouldRemoveDiscount()
    {
        // Arrange
        var orderItem = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Test Product", 100, 1);
        var discount = Discount.CreateByPercentage(20, "Test discount");
        orderItem.ApplyDiscount(discount);

        // Act
        orderItem.RemoveDiscount();

        // Assert
        orderItem.ItemDiscount.Should().BeNull();
        orderItem.HasDiscount().Should().BeFalse();
        orderItem.DiscountAmount.Should().Be(0);
        orderItem.TotalPrice.Should().Be(orderItem.SubTotal);
    }

    [Fact(DisplayName = "Should do nothing when removing discount that doesn't exist")]
    public void RemoveDiscount_WhenNoDiscount_ShouldDoNothing()
    {
        // Arrange
        var orderItem = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Test Product", 100, 1);

        // Act
        orderItem.RemoveDiscount();

        // Assert
        orderItem.ItemDiscount.Should().BeNull();
        orderItem.HasDiscount().Should().BeFalse();
    }

    #endregion

    #region Edge Cases

    [Fact(DisplayName = "Should handle multiple discount applications correctly")]
    public void ApplyDiscount_WhenAppliedMultipleTimes_ShouldReplaceDiscount()
    {
        // Arrange
        var orderItem = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Test Product", 100, 1);
        var firstDiscount = Discount.CreateByPercentage(10, "First discount");
        var secondDiscount = Discount.CreateByPercentage(20, "Second discount");

        // Act
        orderItem.ApplyDiscount(firstDiscount);
        orderItem.ApplyDiscount(secondDiscount);

        // Assert
        orderItem.ItemDiscount.Should().Be(secondDiscount);
        orderItem.DiscountAmount.Should().Be(20); // 20% de 100
        orderItem.TotalPrice.Should().Be(80);
    }

    [Fact(DisplayName = "Should handle very small amounts correctly")]
    public void OrderItem_WhenVerySmallAmounts_ShouldHandleCorrectly()
    {
        // Arrange & Act
        var orderItem = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Test Product", 0.01m, 1);

        // Assert
        orderItem.SubTotal.Should().Be(0.01m);
        orderItem.TotalPrice.Should().Be(0.01m);
    }

    [Fact(DisplayName = "Should handle large amounts correctly")]
    public void OrderItem_WhenLargeAmounts_ShouldHandleCorrectly()
    {
        // Arrange & Act
        var orderItem = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Test Product", 999999.99m, 100);

        // Assert
        orderItem.SubTotal.Should().Be(99999999m);
        orderItem.TotalPrice.Should().Be(99999999m);
    }

    #endregion

    #region Interface Implementation Tests

    [Fact(DisplayName = "Should implement IHasDiscount correctly")]
    public void OrderItem_ShouldImplementIHasDiscountCorrectly()
    {
        // Arrange
        var orderItem = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Test Product", 100, 1);

        // Assert - Verificar se implementa a interface
        orderItem.Should().BeAssignableTo<IHasDiscount>();
        orderItem.HasDiscount().Should().BeFalse();
    }

    [Fact(DisplayName = "Should implement IHasQuantity correctly")]
    public void OrderItem_ShouldImplementIHasQuantityCorrectly()
    {
        // Arrange
        var orderItem = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Test Product", 100, 5);

        // Assert - Verificar se implementa a interface
        orderItem.Should().BeAssignableTo<IHasQuantity>();
        orderItem.Quantity.Value.Should().Be(5);
    }

    [Fact(DisplayName = "Should implement IHasUnitPrice correctly")]
    public void OrderItem_ShouldImplementIHasUnitPriceCorrectly()
    {
        // Arrange
        var unitPrice = 99.99m;
        var orderItem = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Test Product", unitPrice, 1);

        // Assert - Verificar se implementa a interface
        orderItem.Should().BeAssignableTo<IHasUnitPrice>();
        orderItem.UnitPrice.Value.Should().Be(unitPrice);
    }

    #endregion
}