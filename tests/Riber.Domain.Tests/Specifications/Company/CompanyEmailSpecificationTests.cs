using Bogus.Extensions.Brazil;
using FluentAssertions;
using Riber.Domain.Enums;
using Riber.Domain.Specifications.Company;
using Riber.Domain.ValueObjects.Email;
using Entity = Riber.Domain.Entities;

namespace Riber.Domain.Tests.Specifications.Company;

public sealed class CompanyEmailSpecificationTests : BaseTest
{
    [Fact(DisplayName = "Should return true for valid email")]
    public void Should_ReturnTrue_ForValidEmail()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var company = CreateDefaultCompany(email);
        var specification = new CompanyEmailSpecification(Email.Standardization(email));
        
        // Act
        var result = specification.IsSatisfiedBy(company);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact(DisplayName = "Should return false for different email")]
    public void Should_ReturnFalse_ForDifferentEmail()
    {
        // Arrange
        var companyEmail = _faker.Internet.Email();
        var differentEmail = _faker.Internet.Email();
        var company = CreateDefaultCompany(companyEmail);
        var specification = new CompanyEmailSpecification(differentEmail);
        
        // Act
        var result = specification.IsSatisfiedBy(company);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact(DisplayName = "Should return false for empty email")]
    public void Should_ReturnFalse_ForEmptyEmail()
    {
        // Arrange
        var company = Entity.Company.Create(
            _faker.Company.CompanyName(),
            _faker.Company.CompanyName(),
            _faker.Company.Cnpj(),
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber("11987654321"),
            TaxIdType.LegalEntityWithCnpj
        );
        
        var specification = new CompanyEmailSpecification(string.Empty);
        
        // Act
        var result = specification.IsSatisfiedBy(company);
        
        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "ToExpression should be compilable")]
    public void ToExpression_Should_BeCompilable()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var specification = new CompanyEmailSpecification(email);
        
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
        var email = _faker.Internet.Email();
        var company = CreateDefaultCompany(email);
        var specification = new CompanyEmailSpecification(email);
        var compiledExpression = specification.ToExpression().Compile();
        
        // Act
        var resultFromMethod = specification.IsSatisfiedBy(company);
        var resultFromExpression = compiledExpression(company);
        
        // Assert
        resultFromMethod.Should().Be(resultFromExpression);
    }
    
    [Theory(DisplayName = "Should work with different email formats")]
    [InlineData("user@domain.com")]
    [InlineData("user.name@domain.com")]
    [InlineData("test123@example.org")]
    [InlineData("contact@company-name.net")]
    public void Should_WorkWith_DifferentEmailFormats(string email)
    {
        // Arrange
        var company = CreateDefaultCompany(email);
        var specification = new CompanyEmailSpecification(Email.Standardization(email));
        
        // Act
        var result = specification.IsSatisfiedBy(company);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact(DisplayName = "Should be case sensitive")]
    public void Should_BeCaseSensitive()
    {
        // Arrange
        var lowerEmail = "test@company.com";
        var upperEmail = "TEST@COMPANY.COM";
        var company = CreateDefaultCompany(lowerEmail);
        var specification = new CompanyEmailSpecification(upperEmail);
        
        // Act
        var result = specification.IsSatisfiedBy(company);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Theory(DisplayName = "Should work with various business email formats")]
    [InlineData("admin@business.com")]
    [InlineData("contact@startup.io")]
    [InlineData("info@company.org")]
    [InlineData("sales@enterprise.net")]
    [InlineData("support@tech-company.com")]
    public void Should_WorkWith_VariousBusinessEmailFormats(string email)
    {
        // Arrange
        var company = CreateDefaultCompany(email);
        var specification = new CompanyEmailSpecification(email);
        
        // Act
        var result = specification.IsSatisfiedBy(company);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact(DisplayName = "Should handle email with whitespace")]
    public void Should_HandleEmail_WithWhitespace()
    {
        // Arrange
        var cleanEmail = "test@company.com";
        var emailWithSpaces = " test@company.com ";
        var company = CreateDefaultCompany(cleanEmail);
        var specification = new CompanyEmailSpecification(emailWithSpaces);
        
        // Act
        var result = specification.IsSatisfiedBy(company);
        
        // Assert
        result.Should().BeFalse(); // Assumindo que não faz trim automático
    }
    
    [Fact(DisplayName = "Should work with international domains")]
    public void Should_WorkWith_InternationalDomains()
    {
        // Arrange
        var email = "empresa@empresa.com.br";
        var company = CreateDefaultCompany(email);
        var specification = new CompanyEmailSpecification(email);
        
        // Act
        var result = specification.IsSatisfiedBy(company);
        
        // Assert
        result.Should().BeTrue();
    }
    
    private Entity.Company CreateDefaultCompany(string? email = null)
        => Entity.Company.Create(
            _faker.Company.CompanyName(),
            _faker.Company.CompanyName(),
            _faker.Company.Cnpj(),
            email ?? _faker.Internet.Email(),
            _faker.Phone.PhoneNumber("11987654321"),
            TaxIdType.LegalEntityWithCnpj
        );
}