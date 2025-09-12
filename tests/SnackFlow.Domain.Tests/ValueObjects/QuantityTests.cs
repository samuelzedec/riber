using FluentAssertions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.ValueObjects.Quantity;
using SnackFlow.Domain.ValueObjects.Quantity.Exceptions;

namespace SnackFlow.Domain.Tests.ValueObjects;

public sealed class QuantityTests : BaseTest
{
    #region Valid Tests

    [Theory(DisplayName = "Should create quantity for valid positive values")]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(999)]
    public void Create_WhenValidPositiveValue_ShouldCreateQuantity(int value)
    {
        // Act
        var result = Quantity.Create(value);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(value);
    }

    [Fact(DisplayName = "Should create quantity One with value 1")]
    public void One_WhenCalled_ShouldReturnQuantityWithValueOne()
    {
        // Act
        var result = Quantity.One();

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(1);
    }

    [Fact(DisplayName = "Should create quantity Zero with value 0")]
    public void Zero_WhenCalled_ShouldReturnQuantityWithValueZero()
    {
        // Act
        var result = Quantity.Zero();

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(0);
    }

    #endregion

    #region Invalid Tests

    [Theory(DisplayName = "Should throw InvalidQuantityException for zero or negative values")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(-100)]
    public void Create_WhenZeroOrNegativeValue_ShouldThrowInvalidQuantityException(int value)
    {
        // Act
        var act = () => Quantity.Create(value);

        // Assert
        act.Should().Throw<InvalidQuantityException>()
           .WithMessage(ErrorMessage.Invalid.Quantity);
    }

    #endregion

    #region Operators Tests

    [Fact(DisplayName = "Should convert Quantity to int using implicit operator")]
    public void ImplicitOperator_WhenConvertingToInt_ShouldReturnQuantityValue()
    {
        // Arrange
        var value = _faker.Random.Int(1, 100);

        // Act
        var quantity = Quantity.Create(value);
        int result = quantity;

        // Assert
        quantity.Should().NotBeNull();
        result.Should().Be(value);
    }

    [Theory(DisplayName = "Should add two quantities correctly")]
    [InlineData(5, 3, 8)]
    [InlineData(10, 15, 25)]
    [InlineData(1, 1, 2)]
    public void AddOperator_WhenAddingTwoQuantities_ShouldReturnCorrectSum(int first, int second, int expected)
    {
        // Arrange
        var quantity1 = Quantity.Create(first);
        var quantity2 = Quantity.Create(second);

        // Act
        var result = quantity1 + quantity2;

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(expected);
    }

    [Theory(DisplayName = "Should subtract two quantities correctly")]
    [InlineData(10, 3, 7)]
    [InlineData(15, 5, 10)]
    [InlineData(20, 1, 19)]
    public void SubtractOperator_WhenSubtractingTwoQuantities_ShouldReturnCorrectDifference(int first, int second, int expected)
    {
        // Arrange
        var quantity1 = Quantity.Create(first);
        var quantity2 = Quantity.Create(second);

        // Act
        var result = quantity1 - quantity2;

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(expected);
    }

    [Theory(DisplayName = "Should multiply quantity by int correctly")]
    [InlineData(5, 3, 15)]
    [InlineData(10, 2, 20)]
    [InlineData(7, 4, 28)]
    public void MultiplyOperator_WhenMultiplyingQuantityByInt_ShouldReturnCorrectProduct(int quantity, int multiplier, int expected)
    {
        // Arrange
        var quantityValue = Quantity.Create(quantity);

        // Act
        var result = quantityValue * multiplier;

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(expected);
    }

    [Fact(DisplayName = "Should throw InvalidQuantityException when subtraction results in zero or negative")]
    public void SubtractOperator_WhenResultIsZeroOrNegative_ShouldThrowInvalidQuantityException()
    {
        // Arrange
        var quantity1 = Quantity.Create(5);
        var quantity2 = Quantity.Create(5);

        // Act
        var act = () => quantity1 - quantity2;

        // Assert
        act.Should().Throw<InvalidQuantityException>();
    }

    #endregion

    #region Methods Tests

    [Theory(DisplayName = "Should add amount correctly using Add method")]
    [InlineData(5, 3, 8)]
    [InlineData(10, 1, 11)]
    [InlineData(1, 9, 10)]
    public void Add_WhenAddingValidAmount_ShouldReturnCorrectQuantity(int original, int amount, int expected)
    {
        // Arrange
        var quantity = Quantity.Create(original);

        // Act
        var result = quantity.Add(amount);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(expected);
    }

    [Theory(DisplayName = "Should subtract amount correctly using Subtract method")]
    [InlineData(10, 3, 7)]
    [InlineData(5, 2, 3)]
    [InlineData(20, 1, 19)]
    public void Subtract_WhenSubtractingValidAmount_ShouldReturnCorrectQuantity(int original, int amount, int expected)
    {
        // Arrange
        var quantity = Quantity.Create(original);

        // Act
        var result = quantity.Subtract(amount);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(expected);
    }

    [Theory(DisplayName = "Should multiply correctly using Multiply method")]
    [InlineData(5, 2, 10)]
    [InlineData(3, 4, 12)]
    [InlineData(10, 3, 30)]
    public void Multiply_WhenMultiplyingByValidFactor_ShouldReturnCorrectQuantity(int original, int factor, int expected)
    {
        // Arrange
        var quantity = Quantity.Create(original);

        // Act
        var result = quantity.Multiply(factor);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(expected);
    }

    [Theory(DisplayName = "Should return true when quantity is greater than value")]
    [InlineData(10, 5, true)]
    [InlineData(5, 10, false)]
    [InlineData(5, 5, false)]
    public void IsGreaterThan_WhenComparingValues_ShouldReturnCorrectResult(int quantityValue, int compareValue, bool expected)
    {
        // Arrange
        var quantity = Quantity.Create(quantityValue);

        // Act
        var result = quantity.IsGreaterThan(compareValue);

        // Assert
        result.Should().Be(expected);
    }

    [Theory(DisplayName = "Should return true when quantity is less than value")]
    [InlineData(5, 10, true)]
    [InlineData(10, 5, false)]
    [InlineData(5, 5, false)]
    public void IsLessThan_WhenComparingValues_ShouldReturnCorrectResult(int quantityValue, int compareValue, bool expected)
    {
        // Arrange
        var quantity = Quantity.Create(quantityValue);

        // Act
        var result = quantity.IsLessThan(compareValue);

        // Assert
        result.Should().Be(expected);
    }

    [Theory(DisplayName = "Should return true when quantity equals value")]
    [InlineData(5, 5, true)]
    [InlineData(10, 5, false)]
    [InlineData(3, 7, false)]
    public void Equals_WhenComparingValues_ShouldReturnCorrectResult(int quantityValue, int compareValue, bool expected)
    {
        // Arrange
        var quantity = Quantity.Create(quantityValue);

        // Act
        var result = quantity.Equals(compareValue);

        // Assert
        result.Should().Be(expected);
    }

    [Theory(DisplayName = "Should throw InvalidQuantityException when Add results in zero or negative")]
    [InlineData(5, -5)]
    [InlineData(5, -10)]
    public void Add_WhenResultIsZeroOrNegative_ShouldThrowInvalidQuantityException(int original, int amount)
    {
        // Arrange
        var quantity = Quantity.Create(original);

        // Act
        var act = () => quantity.Add(amount);

        // Assert
        act.Should().Throw<InvalidQuantityException>();
    }

    [Theory(DisplayName = "Should throw InvalidQuantityException when Subtract results in zero or negative")]
    [InlineData(5, 5)]
    [InlineData(5, 10)]
    public void Subtract_WhenResultIsZeroOrNegative_ShouldThrowInvalidQuantityException(int original, int amount)
    {
        // Arrange
        var quantity = Quantity.Create(original);

        // Act
        var act = () => quantity.Subtract(amount);

        // Assert
        act.Should().Throw<InvalidQuantityException>();
    }

    [Theory(DisplayName = "Should throw InvalidQuantityException when Multiply results in zero or negative")]
    [InlineData(5, 0)]
    [InlineData(5, -1)]
    [InlineData(10, -2)]
    public void Multiply_WhenFactorIsZeroOrNegative_ShouldThrowInvalidQuantityException(int original, int factor)
    {
        // Arrange
        var quantity = Quantity.Create(original);

        // Act
        var act = () => quantity.Multiply(factor);

        // Assert
        act.Should().Throw<InvalidQuantityException>();
    }

    #endregion

    #region Equality Tests

    [Fact(DisplayName = "Should be equal when two quantities have same value")]
    public void Equality_WhenTwoQuantitiesHaveSameValue_ShouldBeEqual()
    {
        // Arrange
        var value = _faker.Random.Int(1, 100);
        var quantity1 = Quantity.Create(value);
        var quantity2 = Quantity.Create(value);

        // Act & Assert
        quantity1.Should().Be(quantity2);
        (quantity1 == quantity2).Should().BeTrue();
        quantity1.GetHashCode().Should().Be(quantity2.GetHashCode());
    }

    [Fact(DisplayName = "Should not be equal when two quantities have different values")]
    public void Equality_WhenTwoQuantitiesHaveDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var quantity1 = Quantity.Create(5);
        var quantity2 = Quantity.Create(10);

        // Act & Assert
        quantity1.Should().NotBe(quantity2);
        (quantity1 == quantity2).Should().BeFalse();
        quantity1.GetHashCode().Should().NotBe(quantity2.GetHashCode());
    }

    #endregion
}