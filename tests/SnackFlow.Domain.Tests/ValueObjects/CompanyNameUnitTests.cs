using FluentAssertions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Exceptions;
using SnackFlow.Domain.ValueObjects.CompanyName;
using SnackFlow.Domain.ValueObjects.CompanyName.Exceptions;

namespace SnackFlow.Domain.Tests.ValueObjects;

public sealed class CompanyNameUnitTests : BaseTest
{
    #region Valid Creation Scenarios

    [Fact(DisplayName = "Should create CompanyName with Name and FantasyName")]
    public void Create_WhenNameAndFantasyName_ShouldCreateCompanyName()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var fantasyName = _faker.Random.String2(5, 20);

        // Act
        var act = () => CompanyName.Create(name, fantasyName);

        // Assert
        act.Should().NotThrow();
        var result = act.Invoke();
        result.Corporate.Should().Be(name);
        result.Fantasy.Should().Be(fantasyName);
    }

    [Fact(DisplayName = "Should create CompanyName with minimum valid length")]
    public void Create_WhenMinimumValidLength_ShouldCreateCompanyName()
    {
        // Arrange
        var name = _faker.Random.String2(CompanyName.MinLength);
        var fantasyName = _faker.Random.String2(CompanyName.MinLength);

        // Act
        var act = () => CompanyName.Create(name, fantasyName);

        // Assert
        act.Should().NotThrow();
        var result = act.Invoke();
        result.Corporate.Should().HaveLength(CompanyName.MinLength);
        result.Fantasy.Should().HaveLength(CompanyName.MinLength);
    }

    [Fact(DisplayName = "Should create CompanyName with maximum valid length")]
    public void Create_WhenMaximumValidLength_ShouldCreateCompanyName()
    {
        // Arrange
        var name = _faker.Random.String2(CompanyName.CorporateMaxLength);
        var fantasyName = _faker.Random.String2(CompanyName.FantasyMaxLength);

        // Act
        var act = () => CompanyName.Create(name, fantasyName);

        // Assert
        act.Should().NotThrow();
        var result = act.Invoke();
        result.Corporate.Should().HaveLength(CompanyName.CorporateMaxLength);
        result.Fantasy.Should().HaveLength(CompanyName.FantasyMaxLength);
    }

    [Fact(DisplayName = "Should trim whitespace from name and fantasy name")]
    public void Create_WhenWhitespaceInNames_ShouldTrimWhitespace()
    {
        // Arrange
        var nameCore = _faker.Random.String2(10);
        var fantasyNameCore = _faker.Random.String2(10);
        var name = $"  {nameCore}  ";
        var fantasyName = $"  {fantasyNameCore}  ";

        // Act
        var result = CompanyName.Create(name, fantasyName);

        // Assert 
        result.Corporate.Should().Be(nameCore);
        result.Fantasy.Should().Be(fantasyNameCore);
        result.Corporate.Should().NotStartWith(" ");
        result.Corporate.Should().NotEndWith(" ");
        result.Fantasy.Should().NotStartWith(" ");
        result.Fantasy.Should().NotEndWith(" ");
    }

    #endregion

    #region Null and Empty Tests

    [Theory(DisplayName = "Should throw InvalidFantasyNameException for empty or null fantasy names")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("  \t  \n  ")]
    public void Create_WhenEmptyOrNullFantasyName_ShouldThrowInvalidFantasyNameException(string invalidFantasyName)
    {
        // Arrange
        var name = _faker.Random.String2(10);

        // Act
        var act = () => CompanyName.Create(name, invalidFantasyName);

        // Assert
        act.Should().Throw<DomainException>();
        act.Should().ThrowExactly<InvalidFantasyNameException>().WithMessage(ErrorMessage.FantasyName.IsNullOrEmpty);
    }

    [Theory(DisplayName = "Should throw InvalidNameException for empty or null names")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("  \t  \n  ")]
    public void Create_WhenEmptyOrNullName_ShouldThrowInvalidNameException(string invalidName)
    {
        // Arrange
        var fantasyName = _faker.Random.String2(10);

        // Act
        var act = () => CompanyName.Create(invalidName, fantasyName);

        // Assert
        act.Should().Throw<DomainException>();
        act.Should().ThrowExactly<InvalidNameCorporateException>().WithMessage(ErrorMessage.Name.IsNullOrEmpty);
    }

    #endregion
    
    #region Name Property Tests

    [Fact(DisplayName = "Should throw exception when name exceeds maximum length")]
    public void Create_WhenNameExceedsMaximumLength_ShouldThrowException()
    {
        //Arrange
        var name = _faker.Random.String2(160);
        var fantasyName = _faker.Random.String2(5, 20);

        // Act
        var result = () => CompanyName.Create(name, fantasyName);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidLengthCorporateNameException>()
            .WithMessage(ErrorMessage.Name.LengthIsInvalid(CompanyName.MinLength, CompanyName.CorporateMaxLength));
    }

    [Fact(DisplayName = "Should throw exception when name exceeds minimum length")]
    public void Create_WhenNameExceedsMinimumLength_ShouldThrowException()
    {
        // Arrange
        var name = _faker.Random.String2(2);
        var fantasyName = _faker.Random.String2(5, 20);

        // Act
        var result = () => CompanyName.Create(name, fantasyName);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidLengthCorporateNameException>()
            .WithMessage(ErrorMessage.Name.LengthIsInvalid(CompanyName.MinLength, CompanyName.CorporateMaxLength));
    }

    [Fact(DisplayName = "Should throw exception when name is empty")]
    public void Create_WhenNameIsEmpty_ShouldThrowException()
    {
        // Arrange
        var name = string.Empty;
        var fantasyName = _faker.Random.String2(5, 20);

        // Act
        var result = () => CompanyName.Create(name, fantasyName);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidNameCorporateException>().WithMessage(ErrorMessage.Name.IsNullOrEmpty);
    }

    #endregion

    #region FantasyName Property Tests

    [Fact(DisplayName = "Should throw exception when fantasy name exceeds maximum length")]
    public void Create_WhenFantasyNameExceedsMaximumLength_ShouldThrowException()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var fantasyName = _faker.Random.String2(160);

        // Act
        var result = () => CompanyName.Create(name, fantasyName);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidFantasyLengthNameException>().WithMessage(
            ErrorMessage.FantasyName.LengthIsInvalid(CompanyName.MinLength, CompanyName.FantasyMaxLength));
    }

    [Fact(DisplayName = "Should throw exception when fantasy name exceeds minimum length")]
    public void Create_WhenFantasyNameExceedsMinimumLength_ShouldThrowException()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var fantasyName = _faker.Random.String2(2);

        // Act
        var result = () => CompanyName.Create(name, fantasyName);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidFantasyLengthNameException>().WithMessage(
            ErrorMessage.FantasyName.LengthIsInvalid(CompanyName.MinLength, CompanyName.FantasyMaxLength));
    }

    [Fact(DisplayName = "Should throw exception when fantasy name is empty")]
    public void Create_WhenFantasyNameIsEmpty_ShouldThrowException()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var fantasyName = string.Empty;

        // Act
        var result = () => CompanyName.Create(name, fantasyName);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidFantasyNameException>().WithMessage(ErrorMessage.FantasyName.IsNullOrEmpty);
    }

    [Fact(DisplayName = "Should return fantasy name value when implicitly converted to string")]
    public void ImplicitConversion_WhenConvertedToString_ShouldReturnFantasyNameValue()
    {
        // Arrange
        var name = _faker.Company.CompanyName();
        var fantasyName = _faker.Random.String2(5, 20);
        var companyName = CompanyName.Create(name, fantasyName);

        // Act
        string result = companyName;

        // Assert
        result.Should().Be(fantasyName);
    }

    #endregion
}