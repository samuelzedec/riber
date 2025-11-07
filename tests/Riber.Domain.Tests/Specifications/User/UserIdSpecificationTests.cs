using Bogus.Extensions.Brazil;
using FluentAssertions;
using Riber.Domain.Enums;
using Riber.Domain.Specifications.User;
using Entity = Riber.Domain.Entities;

namespace Riber.Domain.Tests.Specifications.User;

public sealed class UserIdSpecificationTests : BaseTest
{
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return true for valid id")]
    public void Should_ReturnTrue_ForValidId()
    {
        // Arrange
        var user = CreateDefaultUser();
        var userId = user.Id;
        var specification = new UserIdSpecification(userId);
        
        // Act
        var result = specification.IsSatisfiedBy(user);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false for empty guid")]
    public void Should_ReturnFalse_ForEmptyGuid()
    {
        // Arrange
        var user = CreateDefaultUser();
        var specification = new UserIdSpecification(Guid.Empty);
        
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
        var userId = Guid.NewGuid();
        var specification = new UserIdSpecification(userId);
        
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
        var user = CreateDefaultUser();
        var specification = new UserIdSpecification(user.Id);
        var compiledExpression = specification.ToExpression().Compile();
        
        // Act
        var resultFromMethod = specification.IsSatisfiedBy(user);
        var resultFromExpression = compiledExpression(user);
        
        // Assert
        resultFromMethod.Should().Be(resultFromExpression);
    }
    
    private Domain.Entities.User.User CreateDefaultUser()
        => Domain.Entities.User.User.Create(
            _faker.Person.FullName,
            _faker.Person.Cpf(),
            BusinessPosition.Director
        );
}