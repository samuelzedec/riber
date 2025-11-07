using FluentAssertions;
using Riber.Domain.Constants.Messages.Entities;
using Riber.Domain.Entities.Catalog;
using Riber.Domain.Entities.Catalog.Exceptions;
using Riber.Domain.Exceptions;

namespace Riber.Domain.Tests.Entities.Catalog;

public sealed class ProductCategoryTests : BaseTest
{
    #region Creation Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should create product category successfully with valid data")]
    public void Create_WhenValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = _faker.Commerce.Categories(1).First();
        var description = _faker.Commerce.ProductDescription();
        var code = _faker.Random.AlphaNumeric(3);
        var companyId = Guid.NewGuid();

        // Act
        var result = ProductCategory.Create(name, description, code, companyId);

        // Assert
        result.Should().NotBeNull();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be(name.Trim());
        result.Description.Should().Be(description);
        result.Code.Should().Be(code.ToUpperInvariant());
        result.CompanyId.Should().Be(companyId);
        result.IsActive.Should().BeTrue();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should trim name when creating product category")]
    public void Create_WhenNameHasWhitespaces_ShouldTrimName()
    {
        // Arrange
        var nameWithSpaces = $"  {_faker.Commerce.Categories(1).First()}  ";
        var description = _faker.Commerce.ProductDescription();
        var code = _faker.Random.AlphaNumeric(3);
        var companyId = Guid.NewGuid();

        // Act
        var result = ProductCategory.Create(nameWithSpaces, description, code, companyId);

        // Assert
        result.Name.Should().Be(nameWithSpaces.Trim());
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should convert code to uppercase when creating product category")]
    public void Create_WhenCodeIsLowercase_ShouldConvertToUppercase()
    {
        // Arrange
        var name = _faker.Commerce.Categories(1).First();
        var description = _faker.Commerce.ProductDescription();
        var lowercaseCode = _faker.Random.AlphaNumeric(3).ToLower();
        var companyId = Guid.NewGuid();

        // Act
        var result = ProductCategory.Create(name, description, lowercaseCode, companyId);

        // Assert
        result.Code.Should().Be(lowercaseCode.ToUpperInvariant());
    }

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "Should throw exception when name is null or empty")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Create_WhenNameIsNullOrEmpty_ShouldThrowException(string invalidName)
    {
        // Arrange
        var description = _faker.Commerce.ProductDescription();
        var code = _faker.Random.AlphaNumeric(3);
        var companyId = Guid.NewGuid();

        // Act
        var act = () => ProductCategory.Create(invalidName, description, code, companyId);

        // Assert
        act.Should().Throw<EmptyProductCategoryNameException>()
           .WithMessage(CategoryErrors.NameEmpty);
    }

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "Should throw exception when code is null or empty")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Create_WhenCodeIsNullOrEmpty_ShouldThrowException(string invalidCode)
    {
        // Arrange
        var name = _faker.Commerce.Categories(1).First();
        var description = _faker.Commerce.ProductDescription();
        var companyId = Guid.NewGuid();

        // Act
        var act = () => ProductCategory.Create(invalidCode, name, description, companyId);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage(CategoryErrors.NameEmpty);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should throw exception when company id is empty")]
    public void Create_WhenCompanyIdIsEmpty_ShouldThrowException()
    {
        // Arrange
        var name = _faker.Commerce.Categories(1).First();
        var description = _faker.Commerce.ProductDescription();
        var code = _faker.Random.AlphaNumeric(3);
        var companyId = Guid.Empty;

        // Act
        var act = () => ProductCategory.Create(code, name, description, companyId);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage(CompanyErrors.Invalid);
    }

    #endregion

    #region Update Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should update details successfully with valid data")]
    public void UpdateDetails_WhenValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var category = ProductCategory.Create(
            _faker.Commerce.Categories(1).First(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.AlphaNumeric(3),
            Guid.NewGuid()
        );

        var newName = _faker.Commerce.Categories(1).First();
        var newDescription = _faker.Commerce.ProductDescription();

        // Act
        category.UpdateDetails(newName, newDescription);

        // Assert
        category.Name.Should().Be(newName);
        category.Description.Should().Be(newDescription);
    }

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "Should throw exception when updating with null or empty name")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void UpdateName_WhenNameIsNullOrEmpty_ShouldThrowException(string invalidName)
    {
        // Arrange
        var name = _faker.Commerce.Categories(1).First();
        var description = _faker.Commerce.ProductDescription();
        var code = _faker.Random.AlphaNumeric(3);
        var companyId = Guid.NewGuid();
        var category = ProductCategory.Create(code, name, description, companyId);

        // Act
        var act = () => category.UpdateDetails(invalidName, null);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage(CategoryErrors.NameEmpty);
    }

    #endregion

    #region Status Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should activate product category successfully")]
    public void Activate_WhenCalled_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var category = ProductCategory.Create(
            _faker.Commerce.Categories(1).First(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.AlphaNumeric(3),
            Guid.NewGuid()
        );
        category.Deactivate();

        // Act
        category.Activate();

        // Assert
        category.IsActive.Should().BeTrue();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should deactivate product category successfully")]
    public void Deactivate_WhenCalled_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var category = ProductCategory.Create(
            _faker.Commerce.Categories(1).First(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.AlphaNumeric(3),
            Guid.NewGuid()
        );

        // Act
        category.Deactivate();

        // Assert
        category.IsActive.Should().BeFalse();
    }

    #endregion

    #region Properties Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should have empty products collection when created")]
    public void ProductsReadOnly_WhenCategoryIsCreated_ShouldBeEmpty()
    {
        // Arrange & Act
        var category = ProductCategory.Create(
            _faker.Commerce.Categories(1).First(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.AlphaNumeric(3),
            Guid.NewGuid()
        );

        // Assert
        category.ProductsReadOnly.Should().BeEmpty();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return read-only collection of products")]
    public void ProductsReadOnly_WhenAccessed_ShouldReturnReadOnlyCollection()
    {
        // Arrange
        var category = ProductCategory.Create(
            _faker.Commerce.Categories(1).First(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.AlphaNumeric(3),
            Guid.NewGuid()
        );

        // Act
        var products = category.ProductsReadOnly;

        // Assert
        products.Should().BeAssignableTo<IReadOnlyCollection<Product>>();
    }

    #endregion
}