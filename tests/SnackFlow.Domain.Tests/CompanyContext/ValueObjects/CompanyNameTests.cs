using SnackFlow.Domain.CompanyContext.ValueObjects.CompanyName;
using SnackFlow.Domain.CompanyContext.ValueObjects.CompanyName.Exceptions;
using SnackFlow.Domain.SharedContext.Constants;
using SnackFlow.Domain.SharedContext.Exceptions;
using FluentAssertions;

namespace SnackFlow.Domain.Tests.CompanyContext.ValueObjects;

public class CompanyNameTests : BaseTest
{
    #region Valid Creation Scenarios

    [Fact(DisplayName = "Should create CompanyName with Name and TradingName")]
    public void ShouldCreateCompanyNameWithNameAndTradingName()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var tradingName = _faker.Random.String2(5, 20);

        // Act
        var act = () => CompanyName.Create(name, tradingName);

        // Assert
        act.Should().NotThrow();
        var result = act.Invoke();
        result.Name.Should().Be(name);
        result.TradingName.Should().Be(tradingName);
    }

    [Fact(DisplayName = "Should create CompanyName with minimum valid length")]
    public void ShouldCreateCompanyNameWithMinimumValidLength()
    {
        // Arrange
        var name = _faker.Random.String2(CompanyName.MinLength);
        var tradingName = _faker.Random.String2(CompanyName.MinLength);

        // Act
        var act = () => CompanyName.Create(name, tradingName);

        // Assert
        act.Should().NotThrow();
        var result = act.Invoke();
        result.Name.Should().HaveLength(CompanyName.MinLength);
        result.TradingName.Should().HaveLength(CompanyName.MinLength);
    }

    [Fact(DisplayName = "Should create CompanyName with maximum valid length")]
    public void ShouldCreateCompanyNameWithMaximumValidLength()
    {
        // Arrange
        var name = _faker.Random.String2(CompanyName.NameMaxLength);
        var tradingName = _faker.Random.String2(CompanyName.TradingNameMaxLength);

        // Act
        var act = () => CompanyName.Create(name, tradingName);

        // Assert
        act.Should().NotThrow();
        var result = act.Invoke();
        result.Name.Should().HaveLength(CompanyName.NameMaxLength);
        result.TradingName.Should().HaveLength(CompanyName.TradingNameMaxLength);
    }

    [Fact(DisplayName = "Should trim whitespace from name and trading name")]
    public void ShouldTrimWhitespaceFromNameAndTradingName()
    {
        // Arrange
        var nameCore = _faker.Random.String2(10);
        var tradingNameCore = _faker.Random.String2(10);
        var name = $"  {nameCore}  ";
        var tradingName = $"  {tradingNameCore}  ";

        // Act
        var result = CompanyName.Create(name, tradingName);

        // Assert 
        result.Name.Should().Be(nameCore);
        result.TradingName.Should().Be(tradingNameCore);
        result.Name.Should().NotStartWith(" ");
        result.Name.Should().NotEndWith(" ");
        result.TradingName.Should().NotStartWith(" ");
        result.TradingName.Should().NotEndWith(" ");
    }

    #endregion

    #region Null and Empty Tests

    [Theory(DisplayName = "Should throw InvalidTradingNameException for empty or null trading names")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("  \t  \n  ")]
    public void ShouldThrowInvalidTradingNameExceptionForEmptyOrNullTradingNames(string invalidTradingName)
    {
        // Arrange
        var name = _faker.Random.String2(10);

        // Act
        var act = () => CompanyName.Create(name, invalidTradingName);

        // Assert
        act.Should().Throw<DomainException>();
        act.Should().ThrowExactly<InvalidTradingNameException>().WithMessage(ErrorMessage.TradingName.IsNullOrEmpty);
    }

    [Theory(DisplayName = "Should throw InvalidNameException for empty or null names")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("  \t  \n  ")]
    public void ShouldThrowInvalidNameExceptionForEmptyOrNullNames(string invalidName)
    {
        // Arrange
        var tradingName = _faker.Random.String2(10);

        // Act
        var act = () => CompanyName.Create(invalidName, tradingName);

        // Assert
        act.Should().Throw<DomainException>();
        act.Should().ThrowExactly<InvalidNameException>().WithMessage(ErrorMessage.Name.IsNullOrEmpty);
    }

    #endregion
    
    #region Name Property Tests

    [Fact(DisplayName = "Should throw exception when name exceeds maximum length")]
    public void ShouldThrowExceptionWhenNameExceedsMaximumLength()
    {
        //Arrange
        var name = _faker.Random.String2(160);
        var tradingName = _faker.Random.String2(5, 20);

        // Act
        var result = () => CompanyName.Create(name, tradingName);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidLengthNameException>()
            .WithMessage(ErrorMessage.Name.LengthIsInvalid(CompanyName.MinLength, CompanyName.NameMaxLength));
    }

    [Fact(DisplayName = "Should throw exception when name exceeds minimum length")]
    public void ShouldThrowExceptionWhenNameExceedsMinimumLength()
    {
        // Arrange
        var name = _faker.Random.String2(2);
        var tradingName = _faker.Random.String2(5, 20);

        // Act
        var result = () => CompanyName.Create(name, tradingName);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidLengthNameException>()
            .WithMessage(ErrorMessage.Name.LengthIsInvalid(CompanyName.MinLength, CompanyName.NameMaxLength));
    }

    [Fact(DisplayName = "Should throw exception when name is empty")]
    public void ShouldThrowExceptionWhenNameIsEmpty()
    {
        // Arrange
        var name = string.Empty;
        var tradingName = _faker.Random.String2(5, 20);

        // Act
        var result = () => CompanyName.Create(name, tradingName);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidNameException>().WithMessage(ErrorMessage.Name.IsNullOrEmpty);
    }

    #endregion

    #region TradingName Property Tests

    [Fact(DisplayName = "Should throw exception when trading name exceeds maximum length")]
    public void ShouldThrowExceptionWhenTradingNameExceedsMaximumLength()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var tradingName = _faker.Random.String2(160);

        // Act
        var result = () => CompanyName.Create(name, tradingName);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidTradingLengthNameException>().WithMessage(
            ErrorMessage.TradingName.LengthIsInvalid(CompanyName.MinLength, CompanyName.TradingNameMaxLength));
    }

    [Fact(DisplayName = "Should throw exception when trading name exceeds minimum length")]
    public void ShouldThrowExceptionWhenTradingNameExceedsMinimumLength()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var tradingName = _faker.Random.String2(2);

        // Act
        var result = () => CompanyName.Create(name, tradingName);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidTradingLengthNameException>().WithMessage(
            ErrorMessage.TradingName.LengthIsInvalid(CompanyName.MinLength, CompanyName.TradingNameMaxLength));
    }

    [Fact(DisplayName = "Should throw exception when trading name is empty")]
    public void ShouldThrowExceptionWhenTradingNameIsEmpty()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var tradingName = string.Empty;

        // Act
        var result = () => CompanyName.Create(name, tradingName);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidTradingNameException>().WithMessage(ErrorMessage.TradingName.IsNullOrEmpty);
    }

    [Fact(DisplayName = "Should return trading name value when implicitly converted to string")]
    public void ShouldReturnTradingNameValueWhenImplicitlyConvertedToString()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var tradingName = _faker.Random.String2(5, 20);
        var companyName = CompanyName.Create(name, tradingName);

        // Act
        string result = companyName;

        // Assert
        result.Should().Be(tradingName);
    }

    #endregion
}