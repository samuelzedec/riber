using FluentAssertions;
using Riber.Domain.Specifications.ProductCategory;
using Entity = Riber.Domain.Entities;

namespace Riber.Domain.Tests.Specifications.ProductCategory;

public sealed class ProductCategoryIdSpecificationTests : BaseTest
{
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
    
    private Entity.ProductCategory CreateDefaultProductCategory()
        => Entity.ProductCategory.Create(
            code: _faker.Commerce.Categories(1).First().ToUpperInvariant(),
            name: _faker.Commerce.Department(),
            description: _faker.Lorem.Sentence(),
            companyId: Guid.NewGuid()
        );
}