using FluentAssertions;
using Riber.Domain.Specifications.Image;

namespace Riber.Domain.Tests.Specifications.Image;

public sealed class ImagesReadyForCleanupSpecificationTests : BaseTest
{
    private readonly ImagesReadyForCleanupSpecification _specification = new();

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return true when image is marked for deletion and not deleted yet")]
    public void Should_ReturnTrue_When_MarkedForDeletion_And_NotDeleted()
    {
        // Arrange
        var image = CreateImage();
        image.MarkForDeletion();

        // Act
        var result = _specification.IsSatisfiedBy(image);

        // Arrange
        result.Should().BeTrue();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false when image has been deleted")]
    public void Should_ReturnFalse_When_DeletedAtHasValue()
    {
        // Arrange
        var image = CreateImage();
        image.MarkForDeletion();
        image.DeleteEntity();

        // Act
        var result = _specification.IsSatisfiedBy(image);

        // Arrange
        result.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false when image has not been marked for deletion")]
    public void Should_ReturnFalse_When_NotMarkedForDeletion()
    {
        // Arrange
        var image = CreateImage();

        // Act
        var result = _specification.IsSatisfiedBy(image);

        // Arrange
        result.Should().BeFalse();
    }

    #region Helpers

    private static Domain.Entities.Catalog.Image CreateImage()
        => Domain.Entities.Catalog.Image.Create(
            length: 100000L,
            contentType: "image/png",
            originalName: "test.png"
        );

    #endregion
}