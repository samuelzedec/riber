using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SnackFlow.Domain.Tests;
using SnackFlow.Infrastructure.Persistence.Interceptors;

namespace SnackFlow.Infrastructure.Tests.Persistence.Interceptors;

public class AuditInterceptorIntegrationTests : BaseTest
{
    #region Fields and Setup

    private readonly TestEntity _testEntity;
    private readonly DbContextOptions<TestDbContext> _options;

    public AuditInterceptorIntegrationTests()
    {
        _testEntity = CreateFaker<TestEntity>()
            .CustomInstantiator(f => new TestEntity(Guid.CreateVersion7()))
            .RuleFor(x => x.Name, f => f.Name.FirstName())
            .RuleFor(x => x.Age, f => f.Random.Int(10, 50))
            .Generate();
        
        _options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new AuditInterceptor())
            .Options;
    }

    #endregion

    #region Update Tests

    [Fact(DisplayName = "Saving changes when entity is modified should call UpdateEntity")]
    public void SavingChangesWhenEntityIsModifiedShouldCallUpdateEntity()
    {
        // Arrange
        using var context = new TestDbContext(_options);
        context.TestEntities.Add(_testEntity);
        context.SaveChanges();

        var originalUpdatedAt = _testEntity.UpdatedAt;
        _testEntity.Name = _faker.Person.FullName;

        // Act
        context.SaveChanges();

        // Assert
        _testEntity.UpdatedAt.Should().NotBe(originalUpdatedAt);
        _testEntity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    #endregion

    #region Delete Tests

    [Fact(DisplayName = "Saving changes when entity is deleted should set DeletedAt and keep entity")]
    public void SavingChangesWhenEntityIsDeletedShouldSetDeletedAtAndKeepEntity()
    {
        // Arrange
        using var context = new TestDbContext(_options);
        context.TestEntities.Add(_testEntity);
        context.SaveChanges();
        
        // Act
        context.TestEntities.Remove(_testEntity);
        context.SaveChanges();
        
        // Assert
        _testEntity.DeletedAt.Should().NotBeNull();
        _testEntity.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        
        var entityInDatabase = context
            .TestEntities
            .IgnoreQueryFilters()
            .FirstOrDefault(x => x.Id == _testEntity.Id);
        
        entityInDatabase.Should().NotBeNull();
        entityInDatabase.Should().BeEquivalentTo(_testEntity);
    }

    #endregion

    #region Query Tests

    [Fact(DisplayName = "Querying entity when entity is deleted should not return entity")]
    public void QueryingEntityWhenEntityIsDeletedShouldNotReturnEntity()
    {
        // Arrange
        using var context = new TestDbContext(_options);
        context.TestEntities.Add(_testEntity);
        context.SaveChanges();
        
        // Act
        context.TestEntities.Remove(_testEntity);
        context.SaveChanges();
        
        // Assert
        _testEntity.DeletedAt.Should().NotBeNull();
        _testEntity.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        
        var entityInDatabase = context
            .TestEntities
            .FirstOrDefault(x => x.Id == _testEntity.Id);
        
        entityInDatabase.Should().BeNull();
    }

    #endregion
}