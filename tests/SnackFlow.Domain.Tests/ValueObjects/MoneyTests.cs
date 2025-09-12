using FluentAssertions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.ValueObjects.Money;
using SnackFlow.Domain.ValueObjects.Money.Exceptions;

namespace SnackFlow.Domain.Tests.ValueObjects;

public sealed class MoneyTests : BaseTest
{
   #region Valid Tests

   [Theory(DisplayName = "Should create money for valid positive values")]
   [InlineData(0)]
   [InlineData(10.50)]
   [InlineData(100)]
   [InlineData(999.99)]
   [InlineData(1000000)]
   public void Create_WhenValidValue_ShouldCreateMoney(decimal value)
   {
       // Act
       var result = Money.Create(value);

       // Assert
       result.Should().NotBeNull();
       result.Value.Should().Be(value);
       result.Currency.Should().Be("BRL");
   }

   [Fact(DisplayName = "Should create money with custom currency")]
   public void Create_WhenValidValueWithCustomCurrency_ShouldCreateMoneyWithCorrectCurrency()
   {
       // Arrange
       var value = _faker.Random.Decimal(0, 1000);
       var currency = "USD";

       // Act
       var result = Money.Create(value, currency);

       // Assert
       result.Should().NotBeNull();
       result.Value.Should().Be(value);
       result.Currency.Should().Be(currency);
   }

   [Fact(DisplayName = "Should create zero money")]
   public void Zero_WhenCalled_ShouldReturnMoneyWithZeroValue()
   {
       // Act
       var result = Money.Zero();

       // Assert
       result.Should().NotBeNull();
       result.Value.Should().Be(0);
       result.Currency.Should().Be("BRL");
   }

   #endregion

   #region Invalid Tests

   [Theory(DisplayName = "Should throw InvalidMoneyException for negative values")]
   [InlineData(-0.01)]
   [InlineData(-1)]
   [InlineData(-100)]
   [InlineData(-999.99)]
   public void Create_WhenNegativeValue_ShouldThrowInvalidMoneyException(decimal value)
   {
       // Act
       var act = () => Money.Create(value);

       // Assert
       act.Should().Throw<InvalidMoneyException>()
          .WithMessage(ErrorMessage.Money.NegativeValue);
   }

   #endregion

   #region Operators Tests

   [Fact(DisplayName = "Should convert Money to decimal using implicit operator")]
   public void ImplicitOperator_WhenConvertingToDecimal_ShouldReturnMoneyValue()
   {
       // Arrange
       var value = _faker.Random.Decimal(0, 1000);

       // Act
       var money = Money.Create(value);
       decimal result = money;

       // Assert
       money.Should().NotBeNull();
       result.Should().Be(value);
   }

   [Theory(DisplayName = "Should add two money values with same currency correctly")]
   [InlineData(10.50, 5.25, 15.75)]
   [InlineData(100, 50, 150)]
   [InlineData(0, 25.99, 25.99)]
   public void AddOperator_WhenSameCurrency_ShouldReturnCorrectSum(decimal first, decimal second, decimal expected)
   {
       // Arrange
       var money1 = Money.Create(first);
       var money2 = Money.Create(second);

       // Act
       var result = money1 + money2;

       // Assert
       result.Should().NotBeNull();
       result.Value.Should().Be(expected);
       result.Currency.Should().Be("BRL");
   }

   [Fact(DisplayName = "Should throw InvalidSumException when adding different currencies")]
   public void AddOperator_WhenDifferentCurrencies_ShouldThrowInvalidSumException()
   {
       // Arrange
       var money1 = Money.Create(100);
       var money2 = Money.Create(50, "USD");

       // Act
       var act = () => money1 + money2;

       // Assert
       act.Should().Throw<InvalidSumException>()
          .WithMessage(ErrorMessage.Money.InvalidSum);
   }

   [Theory(DisplayName = "Should subtract two money values with same currency correctly")]
   [InlineData(100, 25, 75)]
   [InlineData(50.75, 10.25, 40.50)]
   [InlineData(25, 25, 0)]
   public void SubtractOperator_WhenSameCurrency_ShouldReturnCorrectDifference(decimal first, decimal second, decimal expected)
   {
       // Arrange
       var money1 = Money.Create(first);
       var money2 = Money.Create(second);

       // Act
       var result = money1 - money2;

       // Assert
       result.Should().NotBeNull();
       result.Value.Should().Be(expected);
       result.Currency.Should().Be("BRL");
   }

   [Fact(DisplayName = "Should throw InvalidSubtractionException when subtracting different currencies")]
   public void SubtractOperator_WhenDifferentCurrencies_ShouldThrowInvalidSubtractionException()
   {
       // Arrange
       var money1 = Money.Create(100);
       var money2 = Money.Create(50, "USD");

       // Act
       var act = () => money1 - money2;

       // Assert
       act.Should().Throw<InvalidSubtractionException>()
          .WithMessage(ErrorMessage.Money.InvalidSubtraction);
   }

   [Fact(DisplayName = "Should throw InvalidMoneyException when subtraction results in negative value")]
   public void SubtractOperator_WhenResultIsNegative_ShouldThrowInvalidMoneyException()
   {
       // Arrange
       var money1 = Money.Create(50);
       var money2 = Money.Create(100);

       // Act
       var act = () => money1 - money2;

       // Assert
       act.Should().Throw<InvalidMoneyException>()
          .WithMessage(ErrorMessage.Money.NegativeValue);
   }

   [Theory(DisplayName = "Should multiply money by int correctly")]
   [InlineData(10.50, 2, 21.00)]
   [InlineData(25, 4, 100)]
   [InlineData(33.33, 3, 99.99)]
   public void MultiplyOperator_WhenMultiplyingByInt_ShouldReturnCorrectProduct(decimal money, int multiplier, decimal expected)
   {
       // Arrange
       var moneyValue = Money.Create(money);

       // Act
       var result = moneyValue * multiplier;

       // Assert
       result.Should().NotBeNull();
       result.Value.Should().Be(expected);
       result.Currency.Should().Be("BRL");
   }

   [Theory(DisplayName = "Should multiply money by decimal correctly")]
   [InlineData(100, 0.5, 50)]
   [InlineData(25, 1.5, 37.5)]
   [InlineData(33.33, 2.0, 66.66)]
   public void MultiplyOperator_WhenMultiplyingByDecimal_ShouldReturnCorrectProduct(decimal money, decimal multiplier, decimal expected)
   {
       // Arrange
       var moneyValue = Money.Create(money);

       // Act
       var result = moneyValue * multiplier;

       // Assert
       result.Should().NotBeNull();
       result.Value.Should().Be(expected);
       result.Currency.Should().Be("BRL");
   }

   [Theory(DisplayName = "Should throw InvalidMoneyException when multiplying by negative int")]
   [InlineData(-1)]
   [InlineData(-5)]
   public void MultiplyOperator_WhenMultiplyingByNegativeInt_ShouldThrowInvalidMoneyException(int multiplier)
   {
       // Arrange
       var money = Money.Create(100);

       // Act
       var act = () => money * multiplier;

       // Assert
       act.Should().Throw<InvalidMoneyException>()
          .WithMessage(ErrorMessage.Money.NegativeValue);
   }

   [Theory(DisplayName = "Should throw InvalidMoneyException when multiplying by negative decimal")]
   [InlineData(-0.5)]
   [InlineData(-2.5)]
   public void MultiplyOperator_WhenMultiplyingByNegativeDecimal_ShouldThrowInvalidMoneyException(decimal multiplier)
   {
       // Arrange
       var money = Money.Create(100);

       // Act
       var act = () => money * multiplier;

       // Assert
       act.Should().Throw<InvalidMoneyException>()
          .WithMessage(ErrorMessage.Money.NegativeValue);
   }

   #endregion

   #region Methods Tests

   [Theory(DisplayName = "Should multiply correctly using Multiply method with int")]
   [InlineData(25, 2, 50)]
   [InlineData(10.50, 3, 31.50)]
   [InlineData(100, 1, 100)]
   public void Multiply_WhenMultiplyingByInt_ShouldReturnCorrectMoney(decimal original, int quantity, decimal expected)
   {
       // Arrange
       var money = Money.Create(original);

       // Act
       var result = money.Multiply(quantity);

       // Assert
       result.Should().NotBeNull();
       result.Value.Should().Be(expected);
       result.Currency.Should().Be("BRL");
   }

   [Theory(DisplayName = "Should multiply correctly using Multiply method with decimal")]
   [InlineData(100, 0.25, 25)]
   [InlineData(50, 1.5, 75)]
   [InlineData(33.33, 2.0, 66.66)]
   public void Multiply_WhenMultiplyingByDecimal_ShouldReturnCorrectMoney(decimal original, decimal factor, decimal expected)
   {
       // Arrange
       var money = Money.Create(original);

       // Act
       var result = money.Multiply(factor);

       // Assert
       result.Should().NotBeNull();
       result.Value.Should().Be(expected);
       result.Currency.Should().Be("BRL");
   }

   #endregion

   #region Equality Tests

   [Fact(DisplayName = "Should be equal when two money have same value and currency")]
   public void Equality_WhenSameValueAndCurrency_ShouldBeEqual()
   {
       // Arrange
       var value = _faker.Random.Decimal(0, 1000);
       var currency = "USD";
       var money1 = Money.Create(value, currency);
       var money2 = Money.Create(value, currency);

       // Act & Assert
       money1.Should().Be(money2);
       (money1 == money2).Should().BeTrue();
       money1.GetHashCode().Should().Be(money2.GetHashCode());
   }

   [Fact(DisplayName = "Should not be equal when two money have different values")]
   public void Equality_WhenDifferentValues_ShouldNotBeEqual()
   {
       // Arrange
       var money1 = Money.Create(100);
       var money2 = Money.Create(200);

       // Act & Assert
       money1.Should().NotBe(money2);
       (money1 == money2).Should().BeFalse();
   }

   [Fact(DisplayName = "Should not be equal when two money have different currencies")]
   public void Equality_WhenDifferentCurrencies_ShouldNotBeEqual()
   {
       // Arrange
       var money1 = Money.Create(100);
       var money2 = Money.Create(100, "USD");

       // Act & Assert
       money1.Should().NotBe(money2);
       (money1 == money2).Should().BeFalse();
   }

   #endregion
}