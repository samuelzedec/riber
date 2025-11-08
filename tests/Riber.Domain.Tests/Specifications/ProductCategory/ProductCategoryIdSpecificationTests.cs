using FluentAssertions;
using Riber.Domain.Specifications.ProductCategory;

namespace Riber.Domain.Tests.Specifications.ProductCategory;

public sealed class ProductCategoryIdSpecificationTests : BaseTest
{
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return true for matching category id")]
    public void Should_ReturnTrue_ForMatchingCategoryId()
    {
        // Arrange
        var category = CreateDefaultProductCategory();
        var categoryId = category.Id;
        var specification = new ProductCategoryIdSpecification(categoryId);
        
        // Act
        var result = specification.IsSatisfiedBy(category);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false for different category id")]
    public void Should_ReturnFalse_ForDifferentCategoryId()
    {
        // Arrange
        var category = CreateDefaultProductCategory();
        var differentCategoryId = Guid.NewGuid();
        var specification = new ProductCategoryIdSpecification(differentCategoryId);
        
        // Act
        var result = specification.IsSatisfiedBy(category);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false for empty guid")]
    public void Should_ReturnFalse_ForEmptyGuid()
    {
        // Arrange
        var category = CreateDefaultProductCategory();
        var specification = new ProductCategoryIdSpecification(Guid.Empty);
        
        // Act
        var result = specification.IsSatisfiedBy(category);
        
        // Assert
        result.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "ToExpression should be compilable")]
    public void ToExpression_Should_BeCompilable()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var specification = new ProductCategoryIdSpecification(categoryId);
        
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
        var category = CreateDefaultProductCategory();
        var specification = new ProductCategoryIdSpecification(category.Id);
        var compiledExpression = specification.ToExpression().Compile();
        
        // Act
        var resultFromMethod = specification.IsSatisfiedBy(category);
        var resultFromExpression = compiledExpression(category);
        
        // Assert
        resultFromMethod.Should().Be(resultFromExpression);
    }
    
    private Domain.Entities.Catalog.ProductCategory CreateDefaultProductCategory()
        => Domain.Entities.Catalog.ProductCategory.Create(
            code: _faker.Commerce.Categories(1).First().ToUpperInvariant(),
            name: _faker.Commerce.Department(),
            description: _faker.Lorem.Sentence(),
            companyId: Guid.NewGuid()
        );
}