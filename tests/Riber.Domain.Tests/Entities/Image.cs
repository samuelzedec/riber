using FluentAssertions;
using Riber.Domain.Constants.Messages.Entities;
using Riber.Domain.Exceptions;

namespace Riber.Domain.Tests.Entities;

public sealed class Image : BaseTest
{
    #region Create

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should create image when parameters are valid")]
    public void CreateImage_WhenParametersAreValid_ShouldCreateSuccessfully()
    {
        // Arrange
        var image = CreateFaker<Domain.Entities.Image>()
            .CustomInstantiator(f => Domain.Entities.Image.Create(
                length: f.Random.Long(1_000L, 10_000_000L),
                contentType: f.PickRandom("image/png", "image/jpeg", "image/webp"),
                originalName: f.System.FileName("png"))
            );

        // Act
        var result = image.Generate()!;

        // Assert
        result.MarkedForDeletionAt.Should().BeNull();
        result.ShouldDelete.Should().BeFalse();
        result.ContentType.Should().BeOneOf("image/png", "image/jpeg", "image/webp");
        result.Length.Should().BeInRange(1_000L, 10_000_000L);
        result.OriginalName.Should().NotBeNullOrWhiteSpace();
        result.Id.Should().NotBeEmpty();
        result.Extension.Should().BeOneOf(".png", ".jpeg", ".webp");
        result.Key.Should().NotBeNullOrWhiteSpace();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should throw exception when length is less or equal to zero")]
    public void CreateImage_WhenLengthIsInvalid_ShouldThrowException()
    {
        // Arrange
        var image = CreateFaker<Domain.Entities.Image>()
            .CustomInstantiator(f => Domain.Entities.Image.Create(
                length: f.Random.Long(0L, 0L),
                contentType: f.PickRandom("image/png", "image/jpeg", "image/webp"),
                originalName: f.System.FileName("png"))
            );

        // Act
        var result = () => image.Generate();

        // Assert
        result.Should().ThrowExactly<InvalidLengthImageException>(ImageErrors.Length);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should throw exception when content type is not allowed")]
    public void CreateImage_WhenContentTypeIsInvalid_ShouldThrowException()
    {
        // Arrange
        var image = CreateFaker<Domain.Entities.Image>()
            .CustomInstantiator(f => Domain.Entities.Image.Create(
                length: f.Random.Long(1_000L, 10_000_000L),
                contentType: "image/gif",
                originalName: f.System.FileName("png"))
            );

        // Act
        var result = () => image.Generate();

        // Assert
        result.Should().ThrowExactly<InvalidTypeImageException>(ImageErrors.Type);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should throw exception when original name is null or empty")]
    public void CreateImage_WhenOriginalNameIsEmpty_ShouldThrowException()
    {
        // Arrange
        var image = CreateFaker<Domain.Entities.Image>()
            .CustomInstantiator(f => Domain.Entities.Image.Create(
                length: f.Random.Long(1_000L, 10_000_000L),
                contentType: f.PickRandom("image/png", "image/jpeg", "image/webp"),
                originalName: string.Empty));

        // Act
        var result = () => image.Generate();

        // Assert
        result.Should().ThrowExactly<InvalidImageException>(ImageErrors.NameEmpty);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should throw exception when original name has no extension")]
    public void CreateImage_WhenOriginalNameHasNoExtension_ShouldThrowException()
    {
        // Arrange
        var image = CreateFaker<Domain.Entities.Image>()
            .CustomInstantiator(f => Domain.Entities.Image.Create(
                length: f.Random.Long(1_000L, 10_000_000L),
                contentType: f.PickRandom("image/png", "image/jpeg", "image/webp"),
                originalName: f.System.FileName("")));

        // Act
        var result = () => image.Generate();

        // Asset
        result.Should().ThrowExactly<InvalidImageException>(ImageErrors.ExtensionEmpty);
    }

    #endregion

    #region MarkForDeletion

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should mark image for deletion and set properties")]
    public void MarkForDeletion_WhenCalled_ShouldSetDeletionFlags()
    {
        // Arrange
        var image = CreateFaker<Domain.Entities.Image>()
            .CustomInstantiator(f => Domain.Entities.Image.Create(
                length: f.Random.Long(1_000L, 10_000_000L),
                contentType: f.PickRandom("image/png", "image/jpeg", "image/webp"),
                originalName: f.System.FileName("png"))
            ).Generate();

        // Act
        image.MarkForDeletion();

        // Assert
        image.MarkedForDeletionAt.Should().NotBeNull();
        image.ShouldDelete.Should().BeTrue();
    }


    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return true when image is marked for deletion")]
    public void IsMarkedForDeletion_WhenMarked_ShouldReturnTrue()
    {
        // Arrange
        var image = CreateFaker<Domain.Entities.Image>()
            .CustomInstantiator(f => Domain.Entities.Image.Create(
                length: f.Random.Long(1_000L, 10_000_000L),
                contentType: f.PickRandom("image/png", "image/jpeg", "image/webp"),
                originalName: f.System.FileName("png"))
            ).Generate();

        // Act
        image.MarkForDeletion();
        var result = image.IsMarkedForDeletion();

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Validation

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return true when content type is allowed")]
    public void IsValidImageType_WhenTypeIsAllowed_ShouldReturnTrue()
    {
        // Arrange
        string[] allowedTypes = ["image/png", "image/jpg", "image/jpeg", "image/webp"];

        // Act
        var result = allowedTypes.Any(Domain.Entities.Image.IsValidImageType);

        // Assert
        result.Should().BeTrue();
    }


    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false when content type is not allowed")]
    public void IsValidImageType_WhenTypeIsNotAllowed_ShouldReturnFalse()
    {
        // Arrange
        string allowedType = "image/gif";

        // Act
        var result = Domain.Entities.Image.IsValidImageType(allowedType);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region ToString & Implicit

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return correct string representation")]
    public void ToString_WhenCalled_ShouldReturnKeyWithExtension()
    {
        // Arrange
        var image = CreateFaker<Domain.Entities.Image>()
            .CustomInstantiator(f => Domain.Entities.Image.Create(
                length: f.Random.Long(1_000L, 10_000_000L),
                contentType: f.PickRandom("image/png", "image/jpeg", "image/webp"),
                originalName: f.System.FileName("png"))
            ).Generate();
        
        // Act
        var result = image.ToString();
        
        // Assert
        result.Should().Be($"{image.Key}{image.Extension}");
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should convert to string implicitly with correct format")]
    public void ImplicitOperatorString_WhenCalled_ShouldReturnCorrectString()
    {
        // Arrange
        var image = CreateFaker<Domain.Entities.Image>()
            .CustomInstantiator(f => Domain.Entities.Image.Create(
                length: f.Random.Long(1_000L, 10_000_000L),
                contentType: f.PickRandom("image/png", "image/jpeg", "image/webp"),
                originalName: f.System.FileName("png"))
            ).Generate();
        
        // Act
        string result = image;
        
        // Assert
        result.Should().Be($"{image.Key}{image.Extension}");
    }
    
    #endregion
}