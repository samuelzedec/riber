using Entity = Riber.Domain.Entities;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Riber.Domain.Abstractions;
using Riber.Domain.Enums;
using Riber.Domain.Specifications.Company;

namespace Riber.Domain.Tests.Specifications.Company;

public sealed class CompanyTaxIdSpecificationTests : BaseTest
{
    [Fact(DisplayName = "Should return true for valid tax id")]
    public void Should_ReturnTrue_ForValidTaxId()
    {
        // Arrange
        var taxId = _faker.Company.Cnpj();
        var company = CreateCompanyDefault(taxId);
        
        var specification = new CompanyTaxIdSpecification(IDocumentValidator.SanitizeStatic(taxId));
        
        // Act
        var result = specification.IsSatisfiedBy(company);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact(DisplayName = "Should return false for different tax id")]
    public void Should_ReturnFalse_ForDifferentTaxId()
    {
        // Arrange
        var companyTaxId = _faker.Company.Cnpj();
        var differentTaxId = _faker.Company.Cnpj();
        
        var company = CreateCompanyDefault(companyTaxId);
        var specification = new CompanyTaxIdSpecification(differentTaxId);
        
        // Act
        var result = specification.IsSatisfiedBy(company);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact(DisplayName = "Should return false for empty tax id")]
    public void Should_ReturnFalse_ForEmptyTaxId()
    {
        // Arrange
        var company = CreateCompanyDefault();
        var specification = new CompanyTaxIdSpecification(string.Empty);
        
        // Act
        var result = specification.IsSatisfiedBy(company);
        
        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "ToExpression should be compilable")]
    public void ToExpression_Should_BeCompilable()
    {
        // Arrange
        var taxId = _faker.Company.Cnpj();
        var specification = new CompanyTaxIdSpecification(taxId);
        
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
        var taxId = _faker.Company.Cnpj();
        var company = CreateCompanyDefault(taxId);
        
        var specification = new CompanyTaxIdSpecification(taxId);
        var compiledExpression = specification.ToExpression().Compile();
        
        // Act
        var resultFromMethod = specification.IsSatisfiedBy(company);
        var resultFromExpression = compiledExpression(company);
        
        // Assert
        resultFromMethod.Should().Be(resultFromExpression);
    }
    
    
    [Fact(DisplayName = "Should work with expression compilation")]
    public void Should_WorkWith_ExpressionCompilation()
    {
        // Arrange
        var taxId1 = _faker.Company.Cnpj();
        var taxId2 = _faker.Company.Cnpj();
        
        var company1 = CreateCompanyDefault(taxId1);
        var company2 = CreateCompanyDefault(taxId2);
        
        var specification = new CompanyTaxIdSpecification(IDocumentValidator.SanitizeStatic(taxId1));
        var predicate = specification.ToExpression().Compile();
        
        // Act
        var result1 = predicate(company1);
        var result2 = predicate(company2);
        
        // Assert
        result1.Should().BeTrue();
        result2.Should().BeFalse();
    }
    
    [Fact(DisplayName = "Should handle tax id with whitespace")]
    public void Should_HandleTaxId_WithWhitespace()
    {
        // Arrange
        var cleanTaxId = "11222333000181";
        var taxIdWithSpaces = " 11222333000181 ";
        
        var company = CreateCompanyDefault(cleanTaxId);
        var specification = new CompanyTaxIdSpecification(taxIdWithSpaces);
        
        // Act
        var result = specification.IsSatisfiedBy(company);
        
        // Assert
        result.Should().BeFalse();
    }
    
    private Entity.Company CreateCompanyDefault(string? taxId = null)
        => Entity.Company.Create(
            _faker.Company.CompanyName(),
            _faker.Company.CompanyName(),
            taxId ?? _faker.Company.Cnpj(),
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber("11987654321"),
            TaxIdType.LegalEntityWithCnpj
        );
}