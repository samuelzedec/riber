using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SnackFlow.Domain.Tests;
using SnackFlow.Infrastructure.Persistence.Interceptors;

namespace SnackFlow.Infrastructure.Tests.Persistence.Interceptors;

public class AuditInterceptorTest : BaseTest
{
    private readonly AuditInterceptor _auditInterceptor;
    private readonly TestEntity _testEntity;

    public AuditInterceptorTest()
    {
        _auditInterceptor = new AuditInterceptor();
        _testEntity = CreateFaker<TestEntity>()
            .CustomInstantiator(f => new TestEntity(Guid.CreateVersion7()))
            .RuleFor(x => x.Name, f => f.Name.FirstName())
            .RuleFor(x => x.Age, f => f.Random.Int(10, 50))
            .Generate();
    }
    
    [Fact(DisplayName = "Saving changes when entity is modified should call UpdateEntity")]
    public void SavingChanges_WhenEntityIsModified_ShouldCallUpdateEntity()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(_auditInterceptor)
            .Options;

        using var context = new TestDbContext(options);
        
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

}