using FluentAssertions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.ValueObjects.Phone;
using SnackFlow.Domain.ValueObjects.Phone.Exceptions;

namespace SnackFlow.Domain.Tests.ValueObjects;

public class PhoneUnitTests : BaseTest
{
    #region Valid Tests

    [Fact(DisplayName = "Should return true for valid phone")]
    public void Create_WhenValidPhone_ShouldReturnTrue()
    {
        // Arrange
        var phone = "11987654321"; // Exemplo de número válido (11 dígitos)

        // Act 
        var result = Phone.Create(phone);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(phone);
    }

    #endregion

    #region Invalid Tests

    [Theory(DisplayName = "Should throw PhoneNullOrEmptyException for null or empty phone")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("  \t  \n  ")]
    public void Create_WhenNullOrEmptyPhone_ShouldThrowPhoneNullOrEmptyException(string phone)
    {
        var act = () => Phone.Create(phone);
        act.Should().Throw<PhoneNullOrEmptyException>().WithMessage(ErrorMessage.Phone.IsNullOrEmpty);
    }

    [Theory(DisplayName = "Should throw PhoneFormatInvalidException for invalid phone formats")]
    [InlineData("123")]             // Muito curto
    [InlineData("abcdefghij")]      // Apenas letras
    [InlineData("123456789012345")] // Muito longo
    [InlineData("119999")]          // Incompleto
    public void Create_WhenInvalidPhoneFormat_ShouldThrowPhoneFormatInvalidException(string phone)
    {
        var act = () => Phone.Create(phone);
        act.Should().ThrowExactly<PhoneFormatInvalidException>().WithMessage(ErrorMessage.Phone.FormatInvalid);
    }

    #endregion

    #region Operators Test

    [Fact(DisplayName = "Should convert Phone to string using implicit operator")]
    public void ImplicitOperator_WhenConvertingToString_ShouldReturnFormattedPhone()
    {
        // Arrange
        var phone = "11987654321";
        var phoneExpected = "(11) 98765-4321";

        // Act 
        var result = Phone.Create(phone);
        string resultInString = result;

        // Assert
        result.Should().NotBeNull();
        resultInString.Should().Be(phoneExpected);
    }

    #endregion
}