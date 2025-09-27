using FluentAssertions;
using Riber.Domain.Constants;
using Riber.Domain.Entities;
using Riber.Domain.Exceptions;

namespace Riber.Domain.Tests.Entities;

public sealed class ProductCategoryTests : BaseTest
{
    #region Creation Tests

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

    [Theory(DisplayName = "Should throw ProductCategoryNameNullException when name is invalid")]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenNameIsInvalid_ShouldThrowProductCategoryNameNullException(string invalidName)
    {
        // Arrange
        var description = _faker.Commerce.ProductDescription();
        var code = _faker.Random.AlphaNumeric(3);
        var companyId = Guid.NewGuid();

        // Act
        var act = () => ProductCategory.Create(invalidName, description, code, companyId);

        // Assert
        act.Should().Throw<ProductCategoryNameNullException>()
           .WithMessage(ErrorMessage.Product.CategoryNameIsNull);
    }

    [Theory(DisplayName = "Should throw ProductCategoryCodeNullException when code is invalid")]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenCodeIsInvalid_ShouldThrowProductCategoryCodeNullException(string invalidCode)
    {
        // Arrange
        var name = _faker.Commerce.Categories(1).First();
        var description = _faker.Commerce.ProductDescription();
        var companyId = Guid.NewGuid();

        // Act
        var act = () => ProductCategory.Create(name, description, invalidCode, companyId);

        // Assert
        act.Should().Throw<ProductCategoryCodeNullException>()
           .WithMessage(ErrorMessage.Product.CategoryCodeIsNull);
    }

    [Fact(DisplayName = "Should throw IdentifierNullException when company id is empty")]
    public void Create_WhenCompanyIdIsEmpty_ShouldThrowIdentifierNullException()
    {
        // Arrange
        var name = _faker.Commerce.Categories(1).First();
        var description = _faker.Commerce.ProductDescription();
        var code = _faker.Random.AlphaNumeric(3);
        var emptyCompanyId = Guid.Empty;

        // Act
        var act = () => ProductCategory.Create(name, description, code, emptyCompanyId);

        // Assert
        act.Should().Throw<IdentifierNullException>()
           .WithMessage(ErrorMessage.Invalid.CompanyId);
    }

    #endregion

    #region Update Tests

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

    [Theory(DisplayName = "Should throw ProductCategoryNameNullException when updating with invalid name")]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_WhenNameIsInvalid_ShouldThrowProductCategoryNameNullException(string invalidName)
    {
        // Arrange
        var category = ProductCategory.Create(
            _faker.Commerce.Categories(1).First(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.AlphaNumeric(3),
            Guid.NewGuid()
        );

        var description = _faker.Commerce.ProductDescription();

        // Act
        var act = () => category.UpdateDetails(invalidName, description);

        // Assert
        act.Should().Throw<ProductCategoryNameNullException>()
           .WithMessage(ErrorMessage.Product.CategoryNameIsNull);
    }

    #endregion

    #region Status Tests

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