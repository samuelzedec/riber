using FluentAssertions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Validators.DocumentValidator;
using SnackFlow.Domain.Validators.DocumentValidator.Exceptions;

namespace SnackFlow.Domain.Tests.Validators;

public class CpfValidatorTests : BaseTest
{
    #region Private Properties

    private readonly CpfValidator _validator = new();

    #endregion

    #region Valid CPF Tests

    [Theory(DisplayName = "Should return true for valid CPF")]
    [InlineData("11144477735")]
    [InlineData("111.444.777-35")]
    [InlineData("111 444 777 35")]
    public void ShouldReturnTrueForValidCpf(string cpf)
    {
        // Act & Assert
        var exception = Record.Exception(() => _validator.IsValid(cpf));
        exception.Should().BeNull();
    }

    #endregion

    #region Invalid CPF Tests

    [Fact(DisplayName = "Should throw InvalidCpfException for invalid CPF")]
    public void ShouldThrowInvalidCpfExceptionForInvalidCpf()
    {
        // Act & Assert
        Action act = () => _validator.IsValid("12345678901");
        act.Should().Throw<InvalidCpfException>()
            .WithMessage(ErrorMessage.Cpf.IsInvalid);
    }

    #endregion

    #region Length Validation Tests

    [Theory(DisplayName = "Should throw InvalidCpfException for incorrect CPF length")]
    [InlineData("123")]
    [InlineData("12345")]
    [InlineData("123456789")]
    [InlineData("12345678")]
    [InlineData("123456789012")]
    [InlineData("12345678901234")]
    [InlineData("123456789012345")]
    public void ShouldThrowInvalidCpfExceptionForIncorrectLength(string cpf)
    {
        // Act & Assert
        Action act = () => _validator.IsValid(cpf);
        act.Should().Throw<InvalidLengthCpfException>(ErrorMessage.Cpf.LengthIsInvalid);
    }

    #endregion

    #region Null or Empty Tests

    [Theory(DisplayName = "Should throw InvalidCpfException for null or empty CPF")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("  \t  \n  ")]
    public void ShouldThrowInvalidCpfExceptionForNullOrEmptyCpf(string cpf)
    {
        // Act & Assert
        Action act = () => _validator.IsValid(cpf);
        act.Should().Throw<InvalidCpfException>().WithMessage(ErrorMessage.Cpf.IsNullOrEmpty);
    }

    #endregion

    #region Repeated Digits Tests

    [Theory(DisplayName = "Should throw InvalidCpfException for CPF with all same digits")]
    [InlineData("11111111111")]
    [InlineData("000.000.000-00")]
    [InlineData("222.222.222-22")]
    [InlineData("333.333.333-33")]
    [InlineData("444.444.444-44")]
    [InlineData("555.555.555-55")]
    [InlineData("666.666.666-66")]
    [InlineData("777.777.777-77")]
    [InlineData("888.888.888-88")]
    [InlineData("999.999.999-99")]
    public void ShouldThrowInvalidCpfExceptionForCpfWithAllSameDigits(string cpf)
    {
        // Act & Assert
        Action act = () => _validator.IsValid(cpf);
        act.Should().Throw<InvalidCpfException>()
            .WithMessage(ErrorMessage.Cpf.OnlyRepeatedDigits);
    }

    #endregion

    #region Formatting Tests

    [Fact(DisplayName = "Should validate CPF successfully regardless of formatting")]
    public void ShouldValidateCpfSuccessfullyRegardlessOfFormatting()
    {
        // Arrange 
        var cpfWithoutFormat = "11144477735";
        var expectedFormatting = "111.444.777-35";

        // Act
        var result = CpfValidator.Format(cpfWithoutFormat);

        // Assert
        result.Should().Be(expectedFormatting);
    }

    #endregion
}