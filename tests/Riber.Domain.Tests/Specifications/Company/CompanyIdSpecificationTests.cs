using Bogus.Extensions.Brazil;
using FluentAssertions;
using Riber.Domain.Enums;
using Riber.Domain.Specifications.Company;

namespace Riber.Domain.Tests.Specifications.Company;

public sealed class CompanyIdSpecificationTests : BaseTest
{
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return true for valid id")]
    public void Should_ReturnTrue_ForValidId()
    {
        // Arrange
        var company = CreateDefaultCompany();
        
        var companyId = company.Id;
        var specification = new CompanyIdSpecification(companyId);
        
        // Act
        var result = specification.IsSatisfiedBy(company);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false for different id")]
    public void Should_ReturnFalse_ForDifferentId()
    {
        // Arrange
        var company = CreateDefaultCompany();
        
        var differentId = Guid.NewGuid();
        var specification = new CompanyIdSpecification(differentId);
        
        // Act
        var result = specification.IsSatisfiedBy(company);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false for empty guid")]
    public void Should_ReturnFalse_ForEmptyGuid()
    {
        // Arrange
        var company = CreateDefaultCompany();
        
        var specification = new CompanyIdSpecification(Guid.Empty);
        
        // Act
        var result = specification.IsSatisfiedBy(company);
        
        // Assert
        result.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "ToExpression should be compilable")]
    public void ToExpression_Should_BeCompilable()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var specification = new CompanyIdSpecification(companyId);
        
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
        var company = CreateDefaultCompany();
        
        var specification = new CompanyIdSpecification(company.Id);
        var compiledExpression = specification.ToExpression().Compile();
        
        // Act
        var resultFromMethod = specification.IsSatisfiedBy(company);
        var resultFromExpression = compiledExpression(company);
        
        // Assert
        resultFromMethod.Should().Be(resultFromExpression);
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should work with expression compilation")]
    public void Should_WorkWith_ExpressionCompilation()
    {
        // Arrange
        var company1 = CreateDefaultCompany();
        var company2 = CreateDefaultCompany();
        
        var specification = new CompanyIdSpecification(company1.Id);
        var predicate = specification.ToExpression().Compile();
        
        // Act
        var result1 = predicate(company1);
        var result2 = predicate(company2);
        
        // Assert
        result1.Should().BeTrue();
        result2.Should().BeFalse();
    }
    
    private Domain.Entities.Company.Company CreateDefaultCompany()
        => Domain.Entities.Company.Company.Create(
            _faker.Company.CompanyName(),
            _faker.Company.CompanyName(),
            _faker.Company.Cnpj(),
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber("11987654321"),
            TaxIdType.LegalEntityWithCnpj
        );
}