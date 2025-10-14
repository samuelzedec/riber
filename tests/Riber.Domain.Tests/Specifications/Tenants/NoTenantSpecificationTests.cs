using Entity = Riber.Domain.Entities;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Riber.Domain.Enums;
using Riber.Domain.Specifications.Tenants;

namespace Riber.Domain.Tests.Specifications.Tenants;

public sealed class NoTenantSpecificationTests : BaseTest
{
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return true for user with no tenant")]
    public void Should_ReturnTrue_ForUserWithNoTenant()
    {
        // Arrange
        var user = CreateUserDefault(companyId: null);
        var specification = new NoTenantSpecification<Entity.User>();
        
        // Act
        var result = specification.IsSatisfiedBy(user);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false for user with tenant")]
    public void Should_ReturnFalse_ForUserWithTenant()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var user = CreateUserDefault(companyId: tenantId);
        var specification = new NoTenantSpecification<Entity.User>();
        
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
        var specification = new NoTenantSpecification<Entity.User>();
        
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
        var user = CreateUserDefault(companyId: null);
        var specification = new NoTenantSpecification<Entity.User>();
        var compiledExpression = specification.ToExpression().Compile();
        
        // Act
        var resultFromMethod = specification.IsSatisfiedBy(user);
        var resultFromExpression = compiledExpression(user);
        
        // Assert
        resultFromMethod.Should().Be(resultFromExpression);
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should work with expression compilation")]
    public void Should_WorkWith_ExpressionCompilation()
    {
        // Arrange
        var userWithoutTenant = CreateUserDefault(companyId: null);
        var userWithTenant = CreateUserDefault(companyId: Guid.NewGuid());
        var specification = new NoTenantSpecification<Entity.User>();
        var predicate = specification.ToExpression().Compile();
        
        // Act
        var resultWithoutTenant = predicate(userWithoutTenant);
        var resultWithTenant = predicate(userWithTenant);
        
        // Assert
        resultWithoutTenant.Should().BeTrue();
        resultWithTenant.Should().BeFalse();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should distinguish between null and valid tenant")]
    public void Should_DistinguishBetween_NullAndValidTenant()
    {
        // Arrange
        var userWithoutTenant = CreateUserDefault(companyId: null);
        var userWithTenant = CreateUserDefault(companyId: Guid.NewGuid());
        var userWithEmptyGuid = CreateUserDefault(companyId: Guid.Empty);
        var specification = new NoTenantSpecification<Entity.User>();
        
        // Act
        var resultNull = specification.IsSatisfiedBy(userWithoutTenant);
        var resultWithTenant = specification.IsSatisfiedBy(userWithTenant);
        var resultEmptyGuid = specification.IsSatisfiedBy(userWithEmptyGuid);
        
        // Assert
        resultNull.Should().BeTrue();
        resultWithTenant.Should().BeFalse();
        resultEmptyGuid.Should().BeFalse();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should work with users from different positions")]
    public void Should_WorkWith_UsersFromDifferentPositions()
    {
        // Arrange
        var directorUser = CreateUserDefault(companyId: null, position: BusinessPosition.Director);
        var managerUser = CreateUserDefault(companyId: null, position: BusinessPosition.Manager);
        var employeeUser = CreateUserDefault(companyId: null, position: BusinessPosition.Employee);
        var specification = new NoTenantSpecification<Entity.User>();
        
        // Act
        var resultDirector = specification.IsSatisfiedBy(directorUser);
        var resultManager = specification.IsSatisfiedBy(managerUser);
        var resultEmployee = specification.IsSatisfiedBy(employeeUser);
        
        // Assert
        resultDirector.Should().BeTrue();
        resultManager.Should().BeTrue();
        resultEmployee.Should().BeTrue();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should work with mixed tenant scenarios")]
    public void Should_WorkWith_MixedTenantScenarios()
    {
        // Arrange
        var globalUser1 = CreateUserDefault(companyId: null);
        var globalUser2 = CreateUserDefault(companyId: null);
        var tenantUser1 = CreateUserDefault(companyId: Guid.NewGuid());
        var tenantUser2 = CreateUserDefault(companyId: Guid.NewGuid());
        var specification = new NoTenantSpecification<Entity.User>();
        
        // Act
        var resultGlobal1 = specification.IsSatisfiedBy(globalUser1);
        var resultGlobal2 = specification.IsSatisfiedBy(globalUser2);
        var resultTenant1 = specification.IsSatisfiedBy(tenantUser1);
        var resultTenant2 = specification.IsSatisfiedBy(tenantUser2);
        
        // Assert
        resultGlobal1.Should().BeTrue();
        resultGlobal2.Should().BeTrue();
        resultTenant1.Should().BeFalse();
        resultTenant2.Should().BeFalse();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should work consistently across multiple calls")]
    public void Should_WorkConsistently_AcrossMultipleCalls()
    {
        // Arrange
        var userWithoutTenant = CreateUserDefault(companyId: null);
        var userWithTenant = CreateUserDefault(companyId: Guid.NewGuid());
        var specification = new NoTenantSpecification<Entity.User>();
        
        // Act
        var result1 = specification.IsSatisfiedBy(userWithoutTenant);
        var result2 = specification.IsSatisfiedBy(userWithoutTenant);
        var result3 = specification.IsSatisfiedBy(userWithTenant);
        var result4 = specification.IsSatisfiedBy(userWithTenant);
        
        // Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue();
        result3.Should().BeFalse();
        result4.Should().BeFalse();
    }
    
    private Entity.User CreateUserDefault(
        string? name = null,
        string? taxId = null,
        BusinessPosition? position = null,
        Guid? companyId = null)
        => Entity.User.Create(
            name ?? _faker.Person.FullName,
            taxId ?? _faker.Person.Cpf(),
            position ?? BusinessPosition.Director,
            companyId
        );
}