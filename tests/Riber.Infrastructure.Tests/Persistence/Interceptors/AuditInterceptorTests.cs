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
    private readonly ModelTest _modelTest;
    private readonly DbContextOptions<DbContextTest> _options;

    public AuditInterceptorTests()
    {
        _entityTest = CreateFaker<EntityTest>()
            .CustomInstantiator(_ => new EntityTest(Guid.CreateVersion7()))
            .RuleFor(x => x.Name, f => f.Name.FirstName())
            .RuleFor(x => x.Age, f => f.Random.Int(10, 50))
            .Generate();

        _modelTest = CreateFaker<ModelTest>()
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .RuleFor(x => x.Value, f => f.Random.Decimal(0, 1000))
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

    #region BaseModel Update Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Saving changes when model is modified should update UpdatedAt")]
    public void SaveChanges_WhenModelIsModified_ShouldUpdateUpdatedAt()
    {
        // Arrange
        using var context = new DbContextTest(_options);
        context.TestModels.Add(_modelTest);
        context.SaveChanges();

        var originalUpdatedAt = _modelTest.UpdatedAt;
        _modelTest.Description = _faker.Lorem.Sentence();

        // Act
        context.SaveChanges();

        // Assert
        _modelTest.UpdatedAt.Should().NotBe(originalUpdatedAt);
        _modelTest.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    #endregion

    #region BaseModel Delete Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Saving changes when model is deleted should set DeletedAt and keep model")]
    public void SaveChanges_WhenModelIsDeleted_ShouldSetDeletedAtAndKeepModel()
    {
        // Arrange
        using var context = new DbContextTest(_options);
        context.TestModels.Add(_modelTest);
        context.SaveChanges();
        
        // Act
        context.TestModels.Remove(_modelTest);
        context.SaveChanges();
        
        // Assert
        _modelTest.DeletedAt.Should().NotBeNull();
        _modelTest.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        
        var modelInDatabase = context
            .TestModels
            .IgnoreQueryFilters()
            .FirstOrDefault(x => x.Id == _modelTest.Id);
        
        modelInDatabase.Should().NotBeNull();
        modelInDatabase.Should().BeEquivalentTo(_modelTest);
    }

    #endregion

    #region BaseModel Query Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Querying model when model is deleted should not return model")]
    public void Query_WhenModelIsDeleted_ShouldNotReturnModel()
    {
        // Arrange
        using var context = new DbContextTest(_options);
        context.TestModels.Add(_modelTest);
        context.SaveChanges();
        
        // Act
        context.TestModels.Remove(_modelTest);
        context.SaveChanges();
        
        // Assert
        _modelTest.DeletedAt.Should().NotBeNull();
        _modelTest.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        
        var modelInDatabase = context
            .TestModels
            .FirstOrDefault(x => x.Id == _modelTest.Id);
        
        modelInDatabase.Should().BeNull();
    }

    #endregion
}