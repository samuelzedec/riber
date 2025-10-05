using FluentAssertions;
using Riber.Domain.Specifications.Product;
using Entity = Riber.Domain.Entities;

namespace Riber.Domain.Tests.Specifications.Product;

public sealed class ProductByCategoryIdSpecificationTests : BaseTest
{
    [Fact(DisplayName = "Should return true for matching category id")]
    public void Should_ReturnTrue_ForMatchingCategoryId()
    {
        // Arrange
        var product = CreateDefaultProduct();
        var categoryId = product.CategoryId;
        var specification = new ProductByCategoryIdSpecification(categoryId);
        
        // Act
        var result = specification.IsSatisfiedBy(product);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact(DisplayName = "Should return false for different category id")]
    public void Should_ReturnFalse_ForDifferentCategoryId()
    {
        // Arrange
        var product = CreateDefaultProduct();
        var differentCategoryId = Guid.NewGuid();
        var specification = new ProductByCategoryIdSpecification(differentCategoryId);
        
        // Act
        var result = specification.IsSatisfiedBy(product);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact(DisplayName = "Should return false for empty guid")]
    public void Should_ReturnFalse_ForEmptyGuid()
    {
        // Arrange
        var product = CreateDefaultProduct();
        var specification = new ProductByCategoryIdSpecification(Guid.Empty);
        
        // Act
        var result = specification.IsSatisfiedBy(product);
        
        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "ToExpression should be compilable")]
    public void ToExpression_Should_BeCompilable()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var specification = new ProductByCategoryIdSpecification(categoryId);
        
        // Act
        var expression = specification.ToExpression();
        var compiledExpression = expression.Compile();
        
        // Assert
        expression.Should().NotBeNull();
        compiledExpression.Should().NotBeNull();
    }
    
    [Fact(DisplayName = "Compiled expression should work same as IsSatisfiedBy")]
    public void CompiledExpression_Should_WorkSameAs_IsSatisfiedBy()
    {
        // Arrange
        var product = CreateDefaultProduct();
        var specification = new ProductByCategoryIdSpecification(product.CategoryId);
        var compiledExpression = specification.ToExpression().Compile();
        
        // Act
        var resultFromMethod = specification.IsSatisfiedBy(product);
        var resultFromExpression = compiledExpression(product);
        
        // Assert
        resultFromMethod.Should().Be(resultFromExpression);
    }
    
    private Entity.Product CreateDefaultProduct()
        => Entity.Product.Create(
            _faker.Commerce.ProductName(),
            _faker.Commerce.ProductDescription(),
            _faker.Random.Decimal(1, 1000),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.Empty
        );
}