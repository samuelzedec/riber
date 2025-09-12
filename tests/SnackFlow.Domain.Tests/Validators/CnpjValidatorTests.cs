using FluentAssertions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Validators.DocumentValidator;
using SnackFlow.Domain.Validators.DocumentValidator.Exceptions;

namespace SnackFlow.Domain.Tests.Validators;

public sealed class CnpjValidatorTests : BaseTest
{
    #region Private Properties

    private readonly CnpjValidator _validator = new();

    #endregion

    #region Valid CNPJ Tests

    [Theory(DisplayName = "Should return true for valid CNPJ")]
    [InlineData("11222333000181")]
    [InlineData("11.222.333/0001-81")]
    [InlineData("11 222 333 0001 81")]
    [InlineData("34028316000103")]
    [InlineData("34.028.316/0001-03")]
    public void IsValid_WhenValidCnpj_ShouldReturnTrue(string cnpj)
    {
        // Act & Assert
        var exception = Record.Exception(() => _validator.IsValid(cnpj));
        exception.Should().BeNull();
    }

    #endregion

    #region Invalid CNPJ Tests

    [Fact(DisplayName = "Should throw InvalidCnpjException for invalid CNPJ")]
    public void IsValid_WhenInvalidCnpj_ShouldThrowInvalidCnpjException()
    {
        // Act & Assert
        Action act = () => _validator.IsValid("98765432000180");
        act.Should().Throw<InvalidCnpjException>()
            .WithMessage(ErrorMessage.Cnpj.IsInvalid);
    }

    #endregion

    #region Length Validation Tests

    [Theory(DisplayName = "Should throw InvalidCnpjException for incorrect CNPJ length")]
    [InlineData("123")]
    [InlineData("12345678")]
    [InlineData("123456789012")]
    [InlineData("1234567890123")]
    [InlineData("123456789012345")]
    [InlineData("12345678901234567")]
    [InlineData("123456789012345678901")]
    public void IsValid_WhenIncorrectCnpjLength_ShouldThrowInvalidCnpjException(string cnpj)
    {
        // Act & Assert
        Action act = () => _validator.IsValid(cnpj);
        act.Should().Throw<InvalidLengthCnpjException>(ErrorMessage.Cnpj.LengthIsInvalid);
    }

    #endregion

    #region Null or Empty Tests

    [Theory(DisplayName = "Should throw InvalidCnpjException for null or empty CNPJ")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("  \t  \n  ")]
    public void IsValid_WhenNullOrEmptyCnpj_ShouldThrowInvalidCnpjException(string cnpj)
    {
        // Act & Assert
        Action act = () => _validator.IsValid(cnpj);
        act.Should().Throw<InvalidCnpjException>().WithMessage(ErrorMessage.Cnpj.IsNullOrEmpty);
    }

    #endregion

    #region Formatting Tests

    [Fact(DisplayName = "Should validate CNPJ successfully regardless of formatting")]
    public void Format_WhenCnpjWithoutFormat_ShouldReturnFormattedCnpj()
    {
        // Arrange 
        var cnpjWithoutFormat = "11222333000181";
        var expectedFormatting = "11.222.333/0001-81";

        // Act
        var result = CnpjValidator.Format(cnpjWithoutFormat);

        // Assert
        result.Should().Be(expectedFormatting);
    }

    #endregion
}