using Bogus.Extensions.Brazil;
using FluentAssertions;
using Riber.Domain.Abstractions;
using Riber.Domain.Enums;
using Entity = Riber.Domain.Entities;
using Riber.Domain.Specifications.User;

namespace Riber.Domain.Tests.Specifications.User;

public sealed class UserTaxIdSpecificationTests : BaseTest
{
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return true for valid tax id")]
    public void Should_ReturnTrue_ForValidTaxId()
    {
        // Arrange
        var taxId = IDocumentValidator.SanitizeStatic(_faker.Person.Cpf());
        var user = CreateDefaultCompany(taxId);
        var specification = new UserTaxIdSpecification(taxId);
        
        // Act
        var result = specification.IsSatisfiedBy(user);
        
        // Assert
        result.Should().BeTrue();
        taxId.Should().NotBeNull();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false for different tax id")]
    public void Should_ReturnFalse_ForDifferentTaxId()
    {
        // Arrange
        var userTaxId = _faker.Person.Cpf();
        var differentTaxId = _faker.Person.Cpf();
        var user = CreateDefaultCompany(userTaxId);
        var specification = new UserTaxIdSpecification(differentTaxId);
        
        // Act
        var result = specification.IsSatisfiedBy(user);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false for empty tax id")]
    public void Should_ReturnFalse_ForEmptyTaxId()
    {
        // Arrange
        var user = CreateDefaultCompany();
        var specification = new UserTaxIdSpecification(string.Empty);
        
        // Act
        var result = specification.IsSatisfiedBy(user);
        
        // Assert
        result.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "ToExpression should be compilable")]
    public void ToExpression_Should_BeCompilable()
    {
        // Arrange
        var taxId = _faker.Person.Cpf();
        var specification = new UserTaxIdSpecification(taxId);
        
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
        var taxId = _faker.Person.Cpf();
        var user = CreateDefaultCompany(taxId);
        var specification = new UserTaxIdSpecification(taxId);
        var compiledExpression = specification.ToExpression().Compile();
        
        // Act
        var resultFromMethod = specification.IsSatisfiedBy(user);
        var resultFromExpression = compiledExpression(user);
        
        // Assert
        resultFromMethod.Should().Be(resultFromExpression);
    }
    
    private Entity.User CreateDefaultCompany(string? email = null)
        => Entity.User.Create(
            _faker.Person.FullName,
            email ?? _faker.Person.Cpf(),
            BusinessPosition.Director
        );
}