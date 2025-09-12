using FluentAssertions;
using SnackFlow.Domain.Entities;
using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.Tests.Entities;

public sealed class ProductTests : BaseTest
{
    #region Creation Tests

    [Fact(DisplayName = "Should create product successfully with valid data")]
    public void Create_WhenValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = _faker.Commerce.ProductName();
        var description = _faker.Commerce.ProductDescription();
        var price = _faker.Random.Decimal(1, 1000);
        var categoryId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var imageUrl = _faker.Image.PicsumUrl();

        // Act
        var result = Product.Create(
            name,
            description,
            price,
            categoryId,
            companyId,
            imageUrl
        );

        // Assert
        result.Should().NotBeNull();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be(name);
        result.Description.Should().Be(description);
        result.UnitPrice.Value.Should().Be(price);
        result.CategoryId.Should().Be(categoryId);
        result.CompanyId.Should().Be(companyId);
        result.ImageUrl.Should().Be(imageUrl);
        result.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = "Should create product successfully without image")]
    public void Create_WhenValidDataWithoutImage_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = _faker.Commerce.ProductName();
        var description = _faker.Commerce.ProductDescription();
        var price = _faker.Random.Decimal(1, 1000);
        var categoryId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        // Act
        var result = Product.Create(
            name,
            description,
            price,
            categoryId,
            companyId
        );

        // Assert
        result.Should().NotBeNull();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be(name);
        result.Description.Should().Be(description);
        result.UnitPrice.Value.Should().Be(price);
        result.CategoryId.Should().Be(categoryId);
        result.CompanyId.Should().Be(companyId);
        result.ImageUrl.Should().BeNull();
        result.IsActive.Should().BeTrue();
    }

    [Theory(DisplayName = "Should throw exception when name is invalid")]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenInvalidName_ShouldThrowProductNameNullException(string invalidName)
    {
        // Arrange
        var description = _faker.Commerce.ProductDescription();
        var price = _faker.Random.Decimal(1, 1000);
        var categoryId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        // Act & Assert
        var act = () => Product.Create(
            invalidName,
            description,
            price,
            categoryId,
            companyId
        );

        act.Should().Throw<ProductNameNullException>();
    }

    [Theory(DisplayName = "Should throw exception when description is invalid")]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenInvalidDescription_ShouldThrowProductDescriptionNullException(string invalidDescription)
    {
        // Arrange
        var name = _faker.Commerce.ProductName();
        var price = _faker.Random.Decimal(1, 1000);
        var categoryId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        // Act & Assert
        var act = () => Product.Create(
            name,
            invalidDescription,
            price,
            categoryId,
            companyId
        );

        act.Should().Throw<ProductDescriptionNullException>();
    }

    [Fact(DisplayName = "Should throw exception when category id is empty")]
    public void Create_WhenEmptyCategoryId_ShouldThrowIdentifierNullException()
    {
        // Arrange
        var name = _faker.Commerce.ProductName();
        var description = _faker.Commerce.ProductDescription();
        var price = _faker.Random.Decimal(1, 1000);
        var categoryId = Guid.Empty;
        var companyId = Guid.NewGuid();

        // Act & Assert
        var act = () => Product.Create(
            name,
            description,
            price,
            categoryId,
            companyId
        );

        act.Should().Throw<IdentifierNullException>();
    }

    [Fact(DisplayName = "Should throw exception when company id is empty")]
    public void Create_WhenEmptyCompanyId_ShouldThrowIdentifierNullException()
    {
        // Arrange
        var name = _faker.Commerce.ProductName();
        var description = _faker.Commerce.ProductDescription();
        var price = _faker.Random.Decimal(1, 1000);
        var categoryId = Guid.NewGuid();
        var companyId = Guid.Empty;

        // Act & Assert
        var act = () => Product.Create(
            name,
            description,
            price,
            categoryId,
            companyId
        );

        act.Should().Throw<IdentifierNullException>();
    }

    #endregion

    #region Update Tests

    [Fact(DisplayName = "Should update details successfully")]
    public void UpdateDetails_WhenValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var product = Product.Create(
            _faker.Commerce.ProductName(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.Decimal(1, 1000),
            Guid.NewGuid(),
            Guid.NewGuid()
        );

        var newName = _faker.Commerce.ProductName();
        var newDescription = _faker.Commerce.ProductDescription();
        var newPrice = _faker.Random.Decimal(1, 1000);

        // Act
        product.UpdateDetails(newName, newDescription, newPrice);

        // Assert
        product.Name.Should().Be(newName);
        product.Description.Should().Be(newDescription);
        product.UnitPrice.Value.Should().Be(newPrice);
    }

    [Theory(DisplayName = "Should throw exception when updating with invalid name")]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_WhenInvalidName_ShouldThrowProductNameNullException(string invalidName)
    {
        // Arrange
        var product = Product.Create(
            _faker.Commerce.ProductName(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.Decimal(1, 1000),
            Guid.NewGuid(),
            Guid.NewGuid()
        );

        var description = _faker.Commerce.ProductDescription();
        var price = _faker.Random.Decimal(1, 1000);

        // Act & Assert
        var act = () => product.UpdateDetails(invalidName, description, price);

        act.Should().Throw<ProductNameNullException>();
    }

    [Theory(DisplayName = "Should throw exception when updating with invalid description")]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_WhenInvalidDescription_ShouldThrowProductDescriptionNullException(string invalidDescription)
    {
        // Arrange
        var product = Product.Create(
            _faker.Commerce.ProductName(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.Decimal(1, 1000),
            Guid.NewGuid(),
            Guid.NewGuid()
        );

        var name = _faker.Commerce.ProductName();
        var price = _faker.Random.Decimal(1, 1000);

        // Act & Assert
        var act = () => product.UpdateDetails(name, invalidDescription, price);

        act.Should().Throw<ProductDescriptionNullException>();
    }

    [Fact(DisplayName = "Should change category successfully")]
    public void ChangeCategory_WhenValidCategoryId_ShouldChangeSuccessfully()
    {
        // Arrange
        var product = Product.Create(
            _faker.Commerce.ProductName(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.Decimal(1, 1000),
            Guid.NewGuid(),
            Guid.NewGuid()
        );

        var newCategoryId = Guid.NewGuid();

        // Act
        product.ChangeCategory(newCategoryId);

        // Assert
        product.CategoryId.Should().Be(newCategoryId);
    }

    [Fact(DisplayName = "Should throw exception when changing to empty category id")]
    public void ChangeCategory_WhenEmptyCategoryId_ShouldThrowIdentifierNullException()
    {
        // Arrange
        var product = Product.Create(
            _faker.Commerce.ProductName(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.Decimal(1, 1000),
            Guid.NewGuid(),
            Guid.NewGuid()
        );

        // Act & Assert
        var act = () => product.ChangeCategory(Guid.Empty);

        act.Should().Throw<IdentifierNullException>();
    }

    [Fact(DisplayName = "Should update image successfully")]
    public void UpdateImage_WhenValidImageUrl_ShouldUpdateSuccessfully()
    {
        // Arrange
        var product = Product.Create(
            _faker.Commerce.ProductName(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.Decimal(1, 1000),
            Guid.NewGuid(),
            Guid.NewGuid()
        );

        var newImageUrl = _faker.Image.PicsumUrl();

        // Act
        product.UpdateImage(newImageUrl);

        // Assert
        product.ImageUrl.Should().Be(newImageUrl);
    }

    [Fact(DisplayName = "Should update image to null successfully")]
    public void UpdateImage_WhenNullImageUrl_ShouldUpdateSuccessfully()
    {
        // Arrange
        var product = Product.Create(
            _faker.Commerce.ProductName(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.Decimal(1, 1000),
            Guid.NewGuid(),
            Guid.NewGuid(),
            _faker.Image.PicsumUrl()
        );

        // Act
        product.UpdateImage(null);

        // Assert
        product.ImageUrl.Should().BeNull();
    }

    #endregion

    #region Activation Tests

    [Fact(DisplayName = "Should activate product successfully")]
    public void Activate_WhenCalled_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var product = Product.Create(
            _faker.Commerce.ProductName(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.Decimal(1, 1000),
            Guid.NewGuid(),
            Guid.NewGuid()
        );

        product.Deactivate(); // Ensure it's deactivated first

        // Act
        product.Activate();

        // Assert
        product.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = "Should deactivate product successfully")]
    public void Deactivate_WhenCalled_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var product = Product.Create(
            _faker.Commerce.ProductName(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.Decimal(1, 1000),
            Guid.NewGuid(),
            Guid.NewGuid()
        );

        // Act
        product.Deactivate();

        // Assert
        product.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = "Should maintain activation state when activating already active product")]
    public void Activate_WhenAlreadyActive_ShouldMaintainActiveState()
    {
        // Arrange
        var product = Product.Create(
            _faker.Commerce.ProductName(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.Decimal(1, 1000),
            Guid.NewGuid(),
            Guid.NewGuid()
        );

        // Product is created as active by default
        product.IsActive.Should().BeTrue();

        // Act
        product.Activate();

        // Assert
        product.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = "Should maintain deactivation state when deactivating already inactive product")]
    public void Deactivate_WhenAlreadyInactive_ShouldMaintainInactiveState()
    {
        // Arrange
        var product = Product.Create(
            _faker.Commerce.ProductName(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.Decimal(1, 1000),
            Guid.NewGuid(),
            Guid.NewGuid()
        );

        product.Deactivate(); // First deactivation
        product.IsActive.Should().BeFalse();

        // Act
        product.Deactivate(); // Second deactivation

        // Assert
        product.IsActive.Should().BeFalse();
    }

    #endregion
}