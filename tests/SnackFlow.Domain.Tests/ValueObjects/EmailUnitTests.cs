using FluentAssertions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.ValueObjects.Email;
using SnackFlow.Domain.ValueObjects.Email.Exceptions;

namespace SnackFlow.Domain.Tests.ValueObjects;

public class EmailUnitTests : BaseTest
{
    #region Valid Tests

    [Fact(DisplayName = "Should return true for valid email")]
    public void ShouldReturnTrueForValidEmail()
    {
        // Arrange
        var email = _faker.Person.Email;

        // Act 
        var result = Email.Create(email);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(email.ToLower());
    }

    #endregion

    #region Invalid Tests

    [Theory(DisplayName = "Should throw EmailNullOrEmptyException for null or empty email")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("  \t  \n  ")]
    public void ShouldThrowInvalidEmailExceptionForNullOrEmptyEmail(string email)
    {
        var act = () => Email.Create(email);
        act.Should().Throw<EmailNullOrEmptyException>().WithMessage(ErrorMessage.Email.IsNullOrEmpty);
    }

    [Fact(DisplayName = "Should throw EmailFormatInvalidException for invalid email")]
    public void ShouldThrowInvalidEmailExceptionForInvalidEmail()
    {
        // Arrange
        var invalidEmail = $"{_faker.Person.Email}invalid";
        
        // Act
        var act = () => Email.Create(invalidEmail);
        
        // Assert
        act.Should().ThrowExactly<EmailFormatInvalidException>(ErrorMessage.Email.FormatInvalid);
    }

    #endregion

    #region Operators Test

    [Fact(DisplayName = "Should convert Email to string using implicit operator")]
    public void ShouldConvertEmailToStringUsingImplicitOperator()
    {
        // Arrange
        var email = _faker.Person.Email;

        // Act 
        var result = Email.Create(email);
        string resultInString = result;

        // Assert
        result.Should().NotBeNull();
        resultInString.Should().Be(email.ToLower());
    }

    #endregion
}