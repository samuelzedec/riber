using FluentAssertions;
using Riber.Domain.Specifications.ProductCategory;
using Entity = Riber.Domain.Entities;

namespace Riber.Domain.Tests.Specifications.ProductCategory;

public sealed class ProductCategoryCodeSpecificationTests : BaseTest
{
    [Fact(DisplayName = "Should return true for matching category code")]
    public void Should_ReturnTrue_ForMatchingCategoryCode()
    {
        // Arrange
        var category = CreateDefaultProductCategory();
        var code = category.Code;
        var specification = new ProductCategoryCodeSpecification(code);
        
        // Act
        var result = specification.IsSatisfiedBy(category);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact(DisplayName = "Should return false for different category code")]
    public void Should_ReturnFalse_ForDifferentCategoryCode()
    {
        // Arrange
        var category = CreateDefaultProductCategory();
        var differentCode = "DIFFERENT_CODE";
        var specification = new ProductCategoryCodeSpecification(differentCode);
        
        // Act
        var result = specification.IsSatisfiedBy(category);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact(DisplayName = "Should return false for empty string")]
    public void Should_ReturnFalse_ForEmptyString()
    {
        // Arrange
        var category = CreateDefaultProductCategory();
        var specification = new ProductCategoryCodeSpecification(string.Empty);
        
        // Act
        var result = specification.IsSatisfiedBy(category);
        
        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Should be case sensitive")]
    public void Should_BeCaseSensitive()
    {
        // Arrange
        var category = CreateDefaultProductCategory();
        var lowercaseCode = category.Code.ToLowerInvariant();
        var specification = new ProductCategoryCodeSpecification(lowercaseCode);
        
        // Act
        var result = specification.IsSatisfiedBy(category);
        
        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "ToExpression should be compilable")]
    public void ToExpression_Should_BeCompilable()
    {
        // Arrange
        var code = "TEST_CODE";
        var specification = new ProductCategoryCodeSpecification(code);
        
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
        var specification = new ProductCategoryCodeSpecification(category.Code);
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