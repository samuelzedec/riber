using Entity = Riber.Domain.Entities;
using FluentAssertions;
using Riber.Domain.Specifications.Tenants;

namespace Riber.Domain.Tests.Specifications.Tenants;

public sealed class TenantSpecificationTests : BaseTest
{
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return true for valid tenant id")]
    public void Should_ReturnTrue_ForValidTenantId()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var product = CreateProductDefault(companyId: tenantId);
        
        var specification = new TenantSpecification<Domain.Entities.Catalog.Product>(tenantId);
        
        // Act
        var result = specification.IsSatisfiedBy(product);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false for different tenant id")]
    public void Should_ReturnFalse_ForDifferentTenantId()
    {
        // Arrange
        var productTenantId = Guid.NewGuid();
        var differentTenantId = Guid.NewGuid();
        
        var product = CreateProductDefault(companyId: productTenantId);
        var specification = new TenantSpecification<Domain.Entities.Catalog.Product>(differentTenantId);
        
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
        var product = CreateProductDefault();
        var specification = new TenantSpecification<Domain.Entities.Catalog.Product>(Guid.Empty);
        
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
        var tenantId = Guid.NewGuid();
        var specification = new TenantSpecification<Domain.Entities.Catalog.Product>(tenantId);
        
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
        var tenantId = Guid.NewGuid();
        var product = CreateProductDefault(companyId: tenantId);
        
        var specification = new TenantSpecification<Domain.Entities.Catalog.Product>(tenantId);
        var compiledExpression = specification.ToExpression().Compile();
        
        // Act
        var resultFromMethod = specification.IsSatisfiedBy(product);
        var resultFromExpression = compiledExpression(product);
        
        // Assert
        resultFromMethod.Should().Be(resultFromExpression);
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should work with expression compilation")]
    public void Should_WorkWith_ExpressionCompilation()
    {
        // Arrange
        var tenantId1 = Guid.NewGuid();
        var tenantId2 = Guid.NewGuid();
        
        var product1 = CreateProductDefault(companyId: tenantId1);
        var product2 = CreateProductDefault(companyId: tenantId2);
        
        var specification = new TenantSpecification<Domain.Entities.Catalog.Product>(tenantId1);
        var predicate = specification.ToExpression().Compile();
        
        // Act
        var result1 = predicate(product1);
        var result2 = predicate(product2);
        
        // Assert
        result1.Should().BeTrue();
        result2.Should().BeFalse();
    }
    
    [Trait("Category", "Unit")]
    [Theory(DisplayName = "Should work with specific guid patterns")]
    [InlineData("00000000-0000-0000-0000-000000000001")]
    [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff")]
    [InlineData("12345678-1234-1234-1234-123456789abc")]
    [InlineData("abcdef00-1111-2222-3333-444455556666")]
    public void Should_WorkWith_SpecificGuidPatterns(string guidString)
    {
        // Arrange
        var tenantId = Guid.Parse(guidString);
        var product = CreateProductDefault(companyId: tenantId);
        var specification = new TenantSpecification<Domain.Entities.Catalog.Product>(tenantId);
        
        // Act
        var result = specification.IsSatisfiedBy(product);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should handle guid comparison correctly")]
    public void Should_HandleGuidComparison_Correctly()
    {
        // Arrange
        var originalTenantId = Guid.NewGuid();
        var sameTenantId = new Guid(originalTenantId.ToString());
        var differentTenantId = Guid.NewGuid();
        
        var product = CreateProductDefault(companyId: originalTenantId);
        var specSame = new TenantSpecification<Domain.Entities.Catalog.Product>(sameTenantId);
        var specDifferent = new TenantSpecification<Domain.Entities.Catalog.Product>(differentTenantId);
        
        // Act
        var resultSame = specSame.IsSatisfiedBy(product);
        var resultDifferent = specDifferent.IsSatisfiedBy(product);
        
        // Assert
        resultSame.Should().BeTrue();
        resultDifferent.Should().BeFalse();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should work with active and inactive products")]
    public void Should_WorkWith_ActiveAndInactiveProducts()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var activeProduct = CreateProductDefault(companyId: tenantId);
        var inactiveProduct = CreateProductDefault(companyId: tenantId);
        
        // Desativa um produto
        inactiveProduct.Deactivate();
        
        var specification = new TenantSpecification<Domain.Entities.Catalog.Product>(tenantId);
        
        // Act
        var resultActive = specification.IsSatisfiedBy(activeProduct);
        var resultInactive = specification.IsSatisfiedBy(inactiveProduct);
        
        // Assert
        resultActive.Should().BeTrue();
        resultInactive.Should().BeTrue();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should work with products from different categories")]
    public void Should_WorkWith_ProductsFromDifferentCategories()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var categoryId1 = Guid.NewGuid();
        var categoryId2 = Guid.NewGuid();
        
        var product1 = CreateProductDefault(companyId: tenantId, categoryId: categoryId1);
        var product2 = CreateProductDefault(companyId: tenantId, categoryId: categoryId2);
        
        var specification = new TenantSpecification<Domain.Entities.Catalog.Product>(tenantId);
        
        // Act
        var result1 = specification.IsSatisfiedBy(product1);
        var result2 = specification.IsSatisfiedBy(product2);
        
        // Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue();
    }
    
    private Domain.Entities.Catalog.Product CreateProductDefault(
        string? name = null,
        string? description = null,
        decimal? price = null,
        Guid? categoryId = null,
        Guid? companyId = null)
        => Domain.Entities.Catalog.Product.Create(
            name ?? _faker.Commerce.ProductName(),
            description ?? _faker.Commerce.ProductDescription(),
            price ?? _faker.Random.Decimal(10, 1000),
            categoryId ?? Guid.NewGuid(),
            companyId ?? Guid.NewGuid(),
            Guid.Empty
        );
}