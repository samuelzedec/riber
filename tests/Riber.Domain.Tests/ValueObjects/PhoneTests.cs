using FluentAssertions;
using Riber.Domain.Constants.Messages.ValueObjects;
using Riber.Domain.ValueObjects.Phone;
using Riber.Domain.ValueObjects.Phone.Exceptions;

namespace Riber.Domain.Tests.ValueObjects;

public sealed class PhoneTests : BaseTest
{
    #region Valid Tests

    [Fact(DisplayName = "Should return true for valid phone")]
    public void Create_WhenValidPhone_ShouldReturnTrue()
    {
        // Arrange
        var phone = "11987654321";

        // Act 
        var result = Phone.Create(phone);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(phone);
    }

    [Theory(DisplayName = "Should accept phone with valid formatting variations")]
    [InlineData("(11) 98765-4321")]
    [InlineData("11 98765-4321")]
    [InlineData("(11)987654321")]
    [InlineData("11987654321")]
    public void Create_WhenValidPhoneWithDifferentFormats_ShouldReturnPhone(string phone)
    {
        // Act
        var result = Phone.Create(phone);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().MatchRegex(@"^\d{11}$");
    }

    [Theory(DisplayName = "Should accept different DDDs")]
    [InlineData("21987654321")]
    [InlineData("85987654321")]
    [InlineData("47987654321")]
    public void Create_WhenValidDDD_ShouldReturnPhone(string phone)
    {
        // Act
        var result = Phone.Create(phone);

        // Assert
        result.Should().NotBeNull();
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
        // Act
        var act = () => Phone.Create(phone);

        // Assert
        act.Should().Throw<PhoneNullOrEmptyException>().WithMessage(PhoneErrors.Empty);
    }

    [Theory(DisplayName = "Should throw PhoneFormatInvalidException for invalid phone formats")]
    [InlineData("123")]
    [InlineData("abcdefghij")]
    [InlineData("123456789012345")]
    [InlineData("119999")]
    public void Create_WhenInvalidPhoneFormat_ShouldThrowPhoneFormatInvalidException(string phone)
    {
        // Act
        var act = () => Phone.Create(phone);

        // Assert
        act.Should().ThrowExactly<PhoneFormatInvalidException>().WithMessage(PhoneErrors.Format);
    }
    
    [Theory(DisplayName = "Should throw for phone not starting with 9")]
    [InlineData("1188654321")]
    [InlineData("1178654321")]
    public void Create_WhenPhoneNotStartingWith9_ShouldThrowPhoneFormatInvalidException(string phone)
    {
        // Act
        var act = () => Phone.Create(phone);

        // Assert
        act.Should().ThrowExactly<PhoneFormatInvalidException>();
    }

    [Theory(DisplayName = "Should throw for phone with letters mixed with numbers")]
    [InlineData("11987a54321")]
    [InlineData("119876543b1")]
    public void Create_WhenPhoneWithLetters_ShouldThrowPhoneFormatInvalidException(string phone)
    {
        // Act
        var act = () => Phone.Create(phone);

        // Assert
        act.Should().ThrowExactly<PhoneFormatInvalidException>();
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

    #region ToString Tests

    [Fact(DisplayName = "Should format 11-digit phone correctly")]
    public void ToString_When11Digits_ShouldFormatCorrectly()
    {
        // Arrange
        var phone = Phone.Create("11987654321");
        
        // Act
        var result = phone.ToString();
        
        // Assert
        result.Should().Be("(11) 98765-4321");
    }

    #endregion

    #region RemoveFormatting Tests

    [Theory(DisplayName = "Should remove all formatting characters")]
    [InlineData("(11) 98765-4321", "11987654321")]
    [InlineData("11 9 8765-4321", "11987654321")]
    [InlineData("(11)98765-4321", "11987654321")]
    public void RemoveFormatting_WhenFormattedPhone_ShouldReturnOnlyDigits(string input, string expected)
    {
        // Act
        var result = Phone.RemoveFormatting(input);
        
        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Equality Tests

    [Fact(DisplayName = "Should be equal when values are the same")]
    public void Equals_WhenSameValue_ShouldBeTrue()
    {
        // Arrange
        var phone1 = Phone.Create("11987654321");
        var phone2 = Phone.Create("(11) 98765-4321");
        
        // Act & Assert
        phone1.Should().Be(phone2);
        (phone1 == phone2).Should().BeTrue();
    }

    [Fact(DisplayName = "Should not be equal when values are different")]
    public void Equals_WhenDifferentValue_ShouldBeFalse()
    {
        // Arrange
        var phone1 = Phone.Create("11987654321");
        var phone2 = Phone.Create("11987654322");
        
        // Act & Assert
        phone1.Should().NotBe(phone2);
        (phone1 != phone2).Should().BeTrue();
    }

    #endregion

    #region Edge Cases

    [Fact(DisplayName = "Should trim whitespace before validation")]
    public void Create_WhenPhoneWithWhitespace_ShouldTrimAndValidate()
    {
        // Arrange
        var phone = "  11987654321  ";
        
        // Act
        var result = Phone.Create(phone);
        
        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be("11987654321");
    }

    #endregion
}