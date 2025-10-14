using Entity = Riber.Domain.Entities;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Riber.Domain.Enums;
using Riber.Domain.Specifications.Tenants;

namespace Riber.Domain.Tests.Specifications.Tenants;

public sealed class OptionalTenantSpecificationTests : BaseTest
{
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return true for valid tenant id")]
    public void Should_ReturnTrue_ForValidTenantId()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var user = CreateUserDefault(companyId: tenantId);
        var specification = new OptionalTenantSpecification<Entity.User>(tenantId);

        // Act
        var result = specification.IsSatisfiedBy(user);

        // Assert
        result.Should().BeTrue();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false for different tenant id")]
    public void Should_ReturnFalse_ForDifferentTenantId()
    {
        // Arrange
        var userTenantId = Guid.NewGuid();
        var differentTenantId = Guid.NewGuid();
        var user = CreateUserDefault(companyId: userTenantId);
        var specification = new OptionalTenantSpecification<Entity.User>(differentTenantId);

        // Act
        var result = specification.IsSatisfiedBy(user);

        // Assert
        result.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false for empty guid")]
    public void Should_ReturnFalse_ForEmptyGuid()
    {
        // Arrange
        var user = CreateUserDefault();
        var specification = new OptionalTenantSpecification<Entity.User>(Guid.Empty);

        // Act
        var result = specification.IsSatisfiedBy(user);

        // Assert
        result.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false for null tenant id")]
    public void Should_ReturnFalse_ForNullTenantId()
    {
        // Arrange
        var user = CreateUserDefault(companyId: null);
        var specification = new OptionalTenantSpecification<Entity.User>(Guid.NewGuid());

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
        var tenantId = Guid.NewGuid();
        var specification = new OptionalTenantSpecification<Entity.User>(tenantId);

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
        var user = CreateUserDefault(companyId: tenantId);
        var specification = new OptionalTenantSpecification<Entity.User>(tenantId);
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
        var tenantId1 = Guid.NewGuid();
        var tenantId2 = Guid.NewGuid();
        var user1 = CreateUserDefault(companyId: tenantId1);
        var user2 = CreateUserDefault(companyId: tenantId2);
        var specification = new OptionalTenantSpecification<Entity.User>(tenantId1);
        var predicate = specification.ToExpression().Compile();

        // Act
        var result1 = predicate(user1);
        var result2 = predicate(user2);

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
        var user = CreateUserDefault(companyId: tenantId);
        var specification = new OptionalTenantSpecification<Entity.User>(tenantId);

        // Act
        var result = specification.IsSatisfiedBy(user);

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
        var user = CreateUserDefault(companyId: originalTenantId);
        var specSame = new OptionalTenantSpecification<Entity.User>(sameTenantId);
        var specDifferent = new OptionalTenantSpecification<Entity.User>(differentTenantId);

        // Act
        var resultSame = specSame.IsSatisfiedBy(user);
        var resultDifferent = specDifferent.IsSatisfiedBy(user);

        // Assert
        resultSame.Should().BeTrue();
        resultDifferent.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should work with users from different positions")]
    public void Should_WorkWith_UsersFromDifferentPositions()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var directorUser = CreateUserDefault(companyId: tenantId, position: BusinessPosition.Director);
        var managerUser = CreateUserDefault(companyId: tenantId, position: BusinessPosition.Manager);
        var specification = new OptionalTenantSpecification<Entity.User>(tenantId);

        // Act
        var resultDirector = specification.IsSatisfiedBy(directorUser);
        var resultManager = specification.IsSatisfiedBy(managerUser);

        // Assert
        resultDirector.Should().BeTrue();
        resultManager.Should().BeTrue();
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