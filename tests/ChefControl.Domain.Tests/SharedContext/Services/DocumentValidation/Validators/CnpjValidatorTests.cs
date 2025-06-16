using ChefControl.Domain.SharedContext.Constants;
using ChefControl.Domain.SharedContext.Services.DocumentValidation.Exceptions;
using ChefControl.Domain.SharedContext.Services.DocumentValidation.Validators;
using FluentAssertions;

namespace ChefControl.Domain.Tests.SharedContext.Services.DocumentValidation.Validators;

public class CnpjValidatorTests : BaseTest
{
    #region Private Properties

    private readonly CnpjValidator _validator = new();

    #endregion

    [Theory(DisplayName = "Should return true for valid CNPJ")]
    [InlineData("11222333000181")]
    [InlineData("11.222.333/0001-81")]
    [InlineData("11 222 333 0001 81")]
    [InlineData("34028316000103")]
    [InlineData("34.028.316/0001-03")]
    public void ShouldReturnTrueForValidCnpj(string cnpj)
    {
        // Act & Assert
        var exception = Record.Exception(() => _validator.IsValid(cnpj));
        exception.Should().BeNull();
    }

    [Fact(DisplayName = "Should throw InvalidCnpjException for invalid CNPJ")]
    public void ShouldThrowInvalidCnpjExceptionForInvalidCnpj()
    {
        // Act & Assert
        Action act = () => _validator.IsValid("98765432000180");
        act.Should().Throw<InvalidCnpjException>()
            .WithMessage(ErrorMessage.Cnpj.IsInvalid);
    }
    
    [Theory(DisplayName = "Should throw InvalidCnpjException for incorrect CNPJ length")]
    [InlineData("123")]
    [InlineData("12345678")]
    [InlineData("123456789012")]
    [InlineData("1234567890123")]
    [InlineData("123456789012345")]
    [InlineData("12345678901234567")]
    [InlineData("123456789012345678901")]
    public void ShouldThrowInvalidCnpjExceptionForIncorrectLength(string cnpj)
    {
        // Act & Assert
        Action act = () => _validator.IsValid(cnpj);
        act.Should().Throw<InvalidLengthCnpjException>(ErrorMessage.Cnpj.LengthIsInvalid);
    }
    
    [Theory(DisplayName = "Should throw InvalidCnpjException for null or empty CNPJ")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("  \t  \n  ")]
    public void ShouldThrowInvalidCnpjExceptionForNullOrEmptyCnpj(string cnpj)
    {
        // Act & Assert
        Action act = () => _validator.IsValid(cnpj);
        act.Should().Throw<InvalidCnpjException>().WithMessage(ErrorMessage.Cnpj.IsNullOrEmpty);
    }

    [Fact(DisplayName = "Should validate CNPJ successfully regardless of formatting")]
    public void ShouldValidateCnpjSuccessfullyRegardlessOfFormatting()
    {
        // Arrange 
        var cnpjWithoutFormat = "11222333000181";
        var expectedFormatting = "11.222.333/0001-81";

        // Act
        var result = CnpjValidator.Format(cnpjWithoutFormat);
        
        // Assert
        result.Should().Be(expectedFormatting);
    }
}