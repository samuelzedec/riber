using FluentAssertions;
using Riber.Domain.Constants.Messages.ValueObjects;
using Riber.Domain.ValueObjects.Discount;
using Riber.Domain.ValueObjects.Discount.Exceptions;

namespace Riber.Domain.Tests.ValueObjects;

public sealed class DiscountTests : BaseTest
{
    #region Valid Tests

    [Theory(DisplayName = "Should create percentage discount for valid values")]
    [InlineData(0.1, "Desconto mínimo")]
    [InlineData(5, "Desconto cliente")]
    [InlineData(25.5, "Promoção especial")]
    [InlineData(50, "Black Friday")]
    [InlineData(100, "Produto gratuito")]
    public void CreateByPercentage_WhenValidPercentage_ShouldCreateDiscount(decimal percentage, string reason)
    {
        // Act
        var result = Discount.CreateByPercentage(percentage, reason);

        // Assert
        result.Should().NotBeNull();
        result.Percentage.Should().Be(percentage);
        result.FixedAmount.Should().Be(0);
        result.Reason.Should().Be(reason);
    }

    [Theory(DisplayName = "Should create fixed amount discount for valid values")]
    [InlineData(0.01, "Desconto mínimo")]
    [InlineData(10, "Desconto funcionário")]
    [InlineData(50.75, "Compensação")]
    [InlineData(100, "Cortesia")]
    public void CreateByFixedAmount_WhenValidAmount_ShouldCreateDiscount(decimal amount, string reason)
    {
        // Act
        var result = Discount.CreateByFixedAmount(amount, reason);

        // Assert
        result.Should().NotBeNull();
        result.FixedAmount.Should().Be(amount);
        result.Percentage.Should().Be(0);
        result.Reason.Should().Be(reason);
    }

    [Fact(DisplayName = "Should create percentage discount without reason")]
    public void CreateByPercentage_WhenNoReason_ShouldCreateDiscountWithNullReason()
    {
        // Arrange
        var percentage = _faker.Random.Decimal(1, 100);

        // Act
        var result = Discount.CreateByPercentage(percentage);

        // Assert
        result.Should().NotBeNull();
        result.Percentage.Should().Be(percentage);
        result.Reason.Should().BeNull();
    }

    [Fact(DisplayName = "Should create fixed amount discount without reason")]
    public void CreateByFixedAmount_WhenNoReason_ShouldCreateDiscountWithNullReason()
    {
        // Arrange
        var amount = _faker.Random.Decimal(0.01m, 100);

        // Act
        var result = Discount.CreateByFixedAmount(amount);

        // Assert
        result.Should().NotBeNull();
        result.FixedAmount.Should().Be(amount);
        result.Reason.Should().BeNull();
    }

    [Fact(DisplayName = "Should create none discount")]
    public void None_WhenCalled_ShouldReturnDiscountWithZeroValues()
    {
        // Act
        var result = Discount.None();

        // Assert
        result.Should().NotBeNull();
        result.Percentage.Should().Be(0);
        result.FixedAmount.Should().Be(0);
        result.Reason.Should().BeNull();
        result.HasDiscount().Should().BeFalse();
    }

    #endregion

    #region Invalid Tests

    [Theory(DisplayName = "Should throw InvalidPercentageException for invalid percentage values")]
    [InlineData(-0.01)]
    [InlineData(-10)]
    [InlineData(100.01)]
    [InlineData(150)]
    public void CreateByPercentage_WhenInvalidPercentage_ShouldThrowInvalidPercentageException(decimal percentage)
    {
        // Act
        var act = () => Discount.CreateByPercentage(percentage, "Teste");

        // Assert
        act.Should().Throw<InvalidPercentageException>()
           .WithMessage(DiscountErrors.Percentage);
    }

    [Theory(DisplayName = "Should throw InvalidFixedAmountException for invalid fixed amount values")]
    [InlineData(0)]
    [InlineData(-0.01)]
    [InlineData(-10)]
    [InlineData(-100)]
    public void CreateByFixedAmount_WhenInvalidAmount_ShouldThrowInvalidFixedAmountException(decimal amount)
    {
        // Act
        var act = () => Discount.CreateByFixedAmount(amount, "Teste");

        // Assert
        act.Should().Throw<InvalidFixedAmountException>()
           .WithMessage(DiscountErrors.FixedAmount);
    }

    #endregion

    #region Calculate Discount Tests

    [Theory(DisplayName = "Should calculate percentage discount correctly")]
    [InlineData(100, 10, 10)]    // 10% de R$ 100 = R$ 10
    [InlineData(50, 20, 10)]     // 20% de R$ 50 = R$ 10
    [InlineData(200, 25, 50)]    // 25% de R$ 200 = R$ 50
    [InlineData(33.33, 50, 16.665)] // 50% de R$ 33,33 = R$ 16,665
    public void CalculateDiscount_WhenPercentageDiscount_ShouldReturnCorrectAmount(decimal subTotal, decimal percentage, decimal expected)
    {
        // Arrange
        var discount = Discount.CreateByPercentage(percentage, "Teste");

        // Act
        var result = discount.CalculateDiscount(subTotal);

        // Assert
        result.Should().BeApproximately(expected, 0.001m);
    }

    [Theory(DisplayName = "Should calculate fixed amount discount correctly")]
    [InlineData(100, 20, 20)]    // Desconto R$ 20 em R$ 100 = R$ 20
    [InlineData(50, 30, 30)]     // Desconto R$ 30 em R$ 50 = R$ 30
    [InlineData(25, 50, 25)]     // Desconto R$ 50 em R$ 25 = R$ 25 (limitado ao subtotal)
    [InlineData(10, 100, 10)]    // Desconto R$ 100 em R$ 10 = R$ 10 (limitado ao subtotal)
    public void CalculateDiscount_WhenFixedAmountDiscount_ShouldReturnCorrectAmount(decimal subTotal, decimal fixedAmount, decimal expected)
    {
        // Arrange
        var discount = Discount.CreateByFixedAmount(fixedAmount, "Teste");

        // Act
        var result = discount.CalculateDiscount(subTotal);

        // Assert
        result.Should().Be(expected);
    }

    [Theory(DisplayName = "Should return zero when subtotal is zero or negative")]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(-100.50)]
    public void CalculateDiscount_WhenSubTotalIsZeroOrNegative_ShouldReturnZero(decimal subTotal)
    {
        // Arrange
        var percentageDiscount = Discount.CreateByPercentage(20, "Teste");
        var fixedDiscount = Discount.CreateByFixedAmount(50, "Teste");

        // Act
        var percentageResult = percentageDiscount.CalculateDiscount(subTotal);
        var fixedResult = fixedDiscount.CalculateDiscount(subTotal);

        // Assert
        percentageResult.Should().Be(0);
        fixedResult.Should().Be(0);
    }

    [Fact(DisplayName = "Should return zero when no discount is applied")]
    public void CalculateDiscount_WhenNoDiscount_ShouldReturnZero()
    {
        // Arrange
        var discount = Discount.None();
        var subTotal = _faker.Random.Decimal(1, 1000);

        // Act
        var result = discount.CalculateDiscount(subTotal);

        // Assert
        result.Should().Be(0);
    }

    #endregion

    #region Methods Tests

    [Theory(DisplayName = "Should return true when has percentage discount")]
    [InlineData(0.01)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public void HasDiscount_WhenHasPercentageDiscount_ShouldReturnTrue(decimal percentage)
    {
        // Arrange
        var discount = Discount.CreateByPercentage(percentage, "Teste");

        // Act
        var result = discount.HasDiscount();

        // Assert
        result.Should().BeTrue();
    }

    [Theory(DisplayName = "Should return true when has fixed amount discount")]
    [InlineData(0.01)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public void HasDiscount_WhenHasFixedAmountDiscount_ShouldReturnTrue(decimal amount)
    {
        // Arrange
        var discount = Discount.CreateByFixedAmount(amount, "Teste");

        // Act
        var result = discount.HasDiscount();

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Should return false when has no discount")]
    public void HasDiscount_WhenNoDiscount_ShouldReturnFalse()
    {
        // Arrange
        var discount = Discount.None();

        // Act
        var result = discount.HasDiscount();

        // Assert
        result.Should().BeFalse();
    }

    [Theory(DisplayName = "Should return true when is percentage discount")]
    [InlineData(5)]
    [InlineData(25)]
    [InlineData(100)]
    public void IsPercentage_WhenPercentageDiscount_ShouldReturnTrue(decimal percentage)
    {
        // Arrange
        var discount = Discount.CreateByPercentage(percentage, "Teste");

        // Act
        var result = discount.IsPercentage();

        // Assert
        result.Should().BeTrue();
        discount.IsFixedAmount().Should().BeFalse();
    }

    [Theory(DisplayName = "Should return true when is fixed amount discount")]
    [InlineData(10)]
    [InlineData(50.75)]
    [InlineData(100)]
    public void IsFixedAmount_WhenFixedAmountDiscount_ShouldReturnTrue(decimal amount)
    {
        // Arrange
        var discount = Discount.CreateByFixedAmount(amount, "Teste");

        // Act
        var result = discount.IsFixedAmount();

        // Assert
        result.Should().BeTrue();
        discount.IsPercentage().Should().BeFalse();
    }

    [Fact(DisplayName = "Should return false for both when no discount")]
    public void IsPercentageAndIsFixedAmount_WhenNoDiscount_ShouldReturnFalse()
    {
        // Arrange
        var discount = Discount.None();

        // Act & Assert
        discount.IsPercentage().Should().BeFalse();
        discount.IsFixedAmount().Should().BeFalse();
    }

    #endregion

    #region Edge Cases Tests

    [Fact(DisplayName = "Should handle very small subtotal values")]
    public void CalculateDiscount_WhenVerySmallSubTotal_ShouldHandleCorrectly()
    {
        // Arrange
        var discount = Discount.CreateByPercentage(10, "Teste");
        var verySmallSubTotal = 0.01m;

        // Act
        var result = discount.CalculateDiscount(verySmallSubTotal);

        // Assert
        result.Should().Be(0.001m);
    }

    [Fact(DisplayName = "Should handle very large subtotal values")]
    public void CalculateDiscount_WhenVeryLargeSubTotal_ShouldHandleCorrectly()
    {
        // Arrange
        var discount = Discount.CreateByPercentage(5, "Teste");
        var veryLargeSubTotal = 1000000m;

        // Act
        var result = discount.CalculateDiscount(veryLargeSubTotal);

        // Assert
        result.Should().Be(50000m); // 5% de 1.000.000 = 50.000
    }

    #endregion

    #region Equality Tests

    [Fact(DisplayName = "Should be equal when two discounts have same values")]
    public void Equality_WhenSameValues_ShouldBeEqual()
    {
        // Arrange
        var discount1 = Discount.CreateByPercentage(20, "Teste");
        var discount2 = Discount.CreateByPercentage(20, "Teste");

        // Act & Assert
        discount1.Should().Be(discount2);
        (discount1 == discount2).Should().BeTrue();
        discount1.GetHashCode().Should().Be(discount2.GetHashCode());
    }

    [Fact(DisplayName = "Should not be equal when discounts have different values")]
    public void Equality_WhenDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var discount1 = Discount.CreateByPercentage(20, "Teste");
        var discount2 = Discount.CreateByFixedAmount(20, "Teste");

        // Act & Assert
        discount1.Should().NotBe(discount2);
        (discount1 == discount2).Should().BeFalse();
    }

    [Fact(DisplayName = "Should not be equal when discounts have different reasons")]
    public void Equality_WhenDifferentReasons_ShouldNotBeEqual()
    {
        // Arrange
        var discount1 = Discount.CreateByPercentage(20, "Motivo A");
        var discount2 = Discount.CreateByPercentage(20, "Motivo B");

        // Act & Assert
        discount1.Should().NotBe(discount2);
        (discount1 == discount2).Should().BeFalse();
    }

    #endregion
}