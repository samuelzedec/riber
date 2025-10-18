using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Riber.Domain.Tests;
using Riber.Infrastructure.Tests.Persistence.Interceptors.TestModels;
using Riber.Infrastructure.Persistence.Interceptors;

namespace Riber.Infrastructure.Tests.Persistence.Interceptors;

public sealed class AuditInterceptorTests : BaseTest
{
    #region Fields and Setup

    private readonly EntityTest _entityTest;
    private readonly DbContextOptions<DbContextTest> _options;

    public AuditInterceptorTests()
    {
        _entityTest = CreateFaker<EntityTest>()
            .CustomInstantiator(f => new EntityTest(Guid.CreateVersion7()))
            .RuleFor(x => x.Name, f => f.Name.FirstName())
            .RuleFor(x => x.Age, f => f.Random.Int(10, 50))
            .Generate();
        
        _options = new DbContextOptionsBuilder<DbContextTest>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new AuditInterceptor())
            .Options;
    }

    #endregion

    #region Update Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Saving changes when entity is modified should call UpdateEntity")]
    public void SaveChanges_WhenEntityIsModified_ShouldCallUpdateEntity()
    {
        // Arrange
        using var context = new DbContextTest(_options);
        context.TestEntities.Add(_entityTest);
        context.SaveChanges();

        var originalUpdatedAt = _entityTest.UpdatedAt;
        _entityTest.Name = _faker.Person.FullName;

        // Act
        context.SaveChanges();

        // Assert
        _entityTest.UpdatedAt.Should().NotBe(originalUpdatedAt);
        _entityTest.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    #endregion

    #region Delete Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Saving changes when entity is deleted should set DeletedAt and keep entity")]
    public void SaveChanges_WhenEntityIsDeleted_ShouldSetDeletedAtAndKeepEntity()
    {
        // Arrange
        using var context = new DbContextTest(_options);
        context.TestEntities.Add(_entityTest);
        context.SaveChanges();
        
        // Act
        context.TestEntities.Remove(_entityTest);
        context.SaveChanges();
        
        // Assert
        _entityTest.DeletedAt.Should().NotBeNull();
        _entityTest.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        
        var entityInDatabase = context
            .TestEntities
            .IgnoreQueryFilters()
            .FirstOrDefault(x => x.Id == _entityTest.Id);
        
        entityInDatabase.Should().NotBeNull();
        entityInDatabase.Should().BeEquivalentTo(_entityTest);
    }

    #endregion

    #region Query Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Querying entity when entity is deleted should not return entity")]
    public void Query_WhenEntityIsDeleted_ShouldNotReturnEntity()
    {
        // Arrange
        using var context = new DbContextTest(_options);
        context.TestEntities.Add(_entityTest);
        context.SaveChanges();
        
        // Act
        context.TestEntities.Remove(_entityTest);
        context.SaveChanges();
        
        // Assert
        _entityTest.DeletedAt.Should().NotBeNull();
        _entityTest.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        
        var entityInDatabase = context
            .TestEntities
            .FirstOrDefault(x => x.Id == _entityTest.Id);
        
        entityInDatabase.Should().BeNull();
    }

    #endregion
}