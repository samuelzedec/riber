using FluentAssertions;
using Riber.Domain.Constants.Messages.ValueObjects;
using Riber.Domain.ValueObjects.ContentType.Exceptions;

namespace Riber.Domain.Tests.ValueObjects;

public sealed class ContentTypeTests : BaseTest
{
    #region Create

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should create content type when value is valid")]
    public void CreateContentType_WhenValueIsValid_ShouldCreateSuccessfully()
    {
        // Arrange
        var validContentTypes = new[] { "image/png", "image/jpg", "image/jpeg", "image/webp" };
        
        foreach (var validType in validContentTypes)
        {
            // Act
            var result = Domain.ValueObjects.ContentType.ContentType.Create(validType);

            // Assert
            result.Value.Should().Be(validType);
            result.ToString().Should().Be(validType);
        }
    }

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "Should create content type with case insensitive values")]
    [InlineData("IMAGE/PNG")]
    [InlineData("Image/Jpeg")]
    [InlineData("image/WEBP")]
    [InlineData("IMAGE/JPG")]
    public void CreateContentType_WhenValueIsCaseInsensitive_ShouldCreateSuccessfully(string contentType)
    {
        // Act
        var result = Domain.ValueObjects.ContentType.ContentType.Create(contentType);

        // Assert
        result.Value.Should().Be(contentType);
        result.ToString().Should().Be(contentType);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should throw exception when content type is invalid")]
    public void CreateContentType_WhenContentTypeIsInvalid_ShouldThrowException()
    {
        // Arrange
        var invalidContentTypes = new[] { "image/gif", "image/bmp", "text/plain", "application/pdf", "" };

        foreach (var invalidType in invalidContentTypes)
        {
            // Act
            var result = () => Domain.ValueObjects.ContentType.ContentType.Create(invalidType);

            // Assert
            result.Should().ThrowExactly<InvalidContentTypeException>()
                .WithMessage(ContentTypeErrors.Type);
        }
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should throw exception when content type is null")]
    public void CreateContentType_WhenContentTypeIsNull_ShouldThrowException()
    {
        // Act
        var result = () => Domain.ValueObjects.ContentType.ContentType.Create(null!);

        // Assert
        result.Should().ThrowExactly<InvalidContentTypeException>()
            .WithMessage(ContentTypeErrors.Type);
    }

    #endregion

    #region IsValidImageType

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "Should return true for valid image types")]
    [InlineData("image/png")]
    [InlineData("image/jpg")]
    [InlineData("image/jpeg")]
    [InlineData("image/webp")]
    public void IsValidImageType_WhenContentTypeIsValid_ShouldReturnTrue(string contentType)
    {
        // Act
        var result = Domain.ValueObjects.ContentType.ContentType.IsValidImageType(contentType);

        // Assert
        result.Should().BeTrue();
    }

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "Should return true for valid image types case insensitive")]
    [InlineData("IMAGE/PNG")]
    [InlineData("Image/Jpg")]
    [InlineData("image/JPEG")]
    [InlineData("IMAGE/WEBP")]
    public void IsValidImageType_WhenContentTypeIsValidCaseInsensitive_ShouldReturnTrue(string contentType)
    {
        // Act
        var result = Domain.ValueObjects.ContentType.ContentType.IsValidImageType(contentType);

        // Assert
        result.Should().BeTrue();
    }

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "Should return false for invalid image types")]
    [InlineData("image/gif")]
    [InlineData("image/bmp")]
    [InlineData("text/plain")]
    [InlineData("application/pdf")]
    [InlineData("video/mp4")]
    [InlineData("")]
    [InlineData("invalid")]
    public void IsValidImageType_WhenContentTypeIsInvalid_ShouldReturnFalse(string contentType)
    {
        // Act
        var result = Domain.ValueObjects.ContentType.ContentType.IsValidImageType(contentType);

        // Assert
        result.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false when content type is null")]
    public void IsValidImageType_WhenContentTypeIsNull_ShouldReturnFalse()
    {
        // Act
        var result = Domain.ValueObjects.ContentType.ContentType.IsValidImageType(null!);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region ToString & Implicit

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return correct string representation")]
    public void ToString_WhenCalled_ShouldReturnValue()
    {
        // Arrange
        var contentType = Domain.ValueObjects.ContentType.ContentType.Create("image/png");

        // Act
        var result = contentType.ToString();

        // Assert
        result.Should().Be("image/png");
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should convert to string implicitly")]
    public void ImplicitOperatorString_WhenCalled_ShouldReturnValue()
    {
        // Arrange
        var contentType = Domain.ValueObjects.ContentType.ContentType.Create("image/jpeg");

        // Act
        string result = contentType;

        // Assert
        result.Should().Be("image/jpeg");
    }

    #endregion

    #region Equality

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should be equal when values are the same")]
    public void Equality_WhenValuesAreSame_ShouldBeEqual()
    {
        // Arrange
        var contentType1 = Domain.ValueObjects.ContentType.ContentType.Create("image/png");
        var contentType2 = Domain.ValueObjects.ContentType.ContentType.Create("image/png");

        // Act & Assert
        contentType1.Should().Be(contentType2);
        contentType1.Equals(contentType2).Should().BeTrue();
        (contentType1 == contentType2).Should().BeTrue();
        contentType1.GetHashCode().Should().Be(contentType2.GetHashCode());
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should not be equal when values are different")]
    public void Equality_WhenValuesAreDifferent_ShouldNotBeEqual()
    {
        // Arrange
        var contentType1 = Domain.ValueObjects.ContentType.ContentType.Create("image/png");
        var contentType2 = Domain.ValueObjects.ContentType.ContentType.Create("image/jpeg");

        // Act & Assert
        contentType1.Should().NotBe(contentType2);
        contentType1.Equals(contentType2).Should().BeFalse();
        (contentType1 != contentType2).Should().BeTrue();
    }

    #endregion
}