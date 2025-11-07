using FluentAssertions;
using Riber.Domain.Specifications.Product;
using Entity = Riber.Domain.Entities;

namespace Riber.Domain.Tests.Specifications.Product;

public sealed class ProductByIdSpecificationTests : BaseTest
{
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return true for matching product id")]
    public void Should_ReturnTrue_ForMatchingProductId()
    {
        // Arrange
        var product = CreateDefaultProduct();
        var productId = product.Id;
        var specification = new ProductByIdSpecification(productId);
        
        // Act
        var result = specification.IsSatisfiedBy(product);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false for different product id")]
    public void Should_ReturnFalse_ForDifferentProductId()
    {
        // Arrange
        var product = CreateDefaultProduct();
        var differentProductId = Guid.NewGuid();
        var specification = new ProductByIdSpecification(differentProductId);
        
        // Act
        var result = specification.IsSatisfiedBy(product);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false for empty guid")]
    public void Should_ReturnFalse_ForEmptyGuid()
    {
        // Arrange
        var product = CreateDefaultProduct();
        var specification = new ProductByIdSpecification(Guid.Empty);
        
        // Act
        var result = specification.IsSatisfiedBy(product);
        
        // Assert
        result.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "ToExpression should be compilable")]
    public void ToExpression_Should_BeCompilable()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var specification = new ProductByIdSpecification(productId);
        
        // Act
        var expression = specification.ToExpression();
        var compiledExpression = expression.Compile();
        
        // Assert
        expression.Should().NotBeNull();
        compiledExpression.Should().NotBeNull();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Compiled expression should work same as IsSatisfiedBy")]
    public void CompiledExpression_Should_WorkSameAs_IsSatisfiedBy()
    {
        // Arrange
        var product = CreateDefaultProduct();
        var specification = new ProductByIdSpecification(product.Id);
        var compiledExpression = specification.ToExpression().Compile();
        
        // Act
        var resultFromMethod = specification.IsSatisfiedBy(product);
        var resultFromExpression = compiledExpression(product);
        
        // Assert
        resultFromMethod.Should().Be(resultFromExpression);
    }
    
    private Domain.Entities.Catalog.Product CreateDefaultProduct()
        => Domain.Entities.Catalog.Product.Create(
            _faker.Commerce.ProductName(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.Decimal(1, 1000),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.Empty
        );
}

