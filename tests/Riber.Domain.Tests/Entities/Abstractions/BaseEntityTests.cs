using FluentAssertions;
using Riber.Domain.Abstractions;
using Riber.Domain.Entities.Abstractions;

namespace Riber.Domain.Tests.Entities.Abstractions;

public sealed class BaseEntityTests : BaseTest
{
    #region Test Entity Implementation

    private sealed class TestEntity(Guid id) : BaseEntity(id)
    {
        public string Name { get; set; } = string.Empty;
    }

    private sealed class TestDomainEvent : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public string EventType { get; } = "TestEvent";
    }

    #endregion

    #region Creation Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should create entity successfully with valid id")]
    public void Create_WhenValidId_ShouldCreateSuccessfully()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var entity = new TestEntity(id);

        // Assert
        entity.Should().NotBeNull();
        entity.Id.Should().Be(id);
        entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        entity.UpdatedAt.Should().BeNull();
        entity.DeletedAt.Should().BeNull();
        entity.Events().Should().BeEmpty();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should create entity with unique id when different guids provided")]
    public void Create_WhenDifferentIds_ShouldCreateWithUniqueIds()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        // Act
        var entity1 = new TestEntity(id1);
        var entity2 = new TestEntity(id2);

        // Assert
        entity1.Id.Should().NotBe(entity2.Id);
        entity1.Should().NotBe(entity2);
    }

    #endregion

    #region Equality Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should be equal when entities have same id")]
    public void Equals_WhenSameId_ShouldBeEqual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act & Assert
        entity1.Should().Be(entity2);
        entity1.Equals(entity2).Should().BeTrue();
        (entity1 == entity2).Should().BeTrue();
        (entity1 != entity2).Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should not be equal when entities have different ids")]
    public void Equals_WhenDifferentIds_ShouldNotBeEqual()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        // Act & Assert
        entity1.Should().NotBe(entity2);
        entity1.Equals(entity2).Should().BeFalse();
        (entity1 == entity2).Should().BeFalse();
        (entity1 != entity2).Should().BeTrue();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should not be equal when comparing with null")]
    public void Equals_WhenComparingWithNull_ShouldNotBeEqual()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid());
        TestEntity? nullEntity = null;

        // Act & Assert
        entity.Equals(null).Should().BeFalse();
        entity?.Equals(nullEntity).Should().BeFalse();
        (entity is null).Should().BeFalse();
        (entity == nullEntity).Should().BeFalse();
        (entity != null).Should().BeTrue();
        (entity != nullEntity).Should().BeTrue();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should be equal when both entities are null")]
    public void Equals_WhenBothNull_ShouldBeEqual()
    {
        // Arrange
        TestEntity? entity1 = null;
        TestEntity? entity2 = null;

        // Act & Assert
        (entity1 == entity2).Should().BeTrue();
        (entity1 != entity2).Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should not be equal when comparing with different type")]
    public void Equals_WhenDifferentType_ShouldNotBeEqual()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid());
        var otherObject = "string";

        // Act
        var result = entity.Equals(otherObject);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetHashCode Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should have same hash code when entities have same id")]
    public void GetHashCode_WhenSameId_ShouldHaveSameHashCode()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act
        var hashCode1 = entity1.GetHashCode();
        var hashCode2 = entity2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should have different hash codes when entities have different ids")]
    public void GetHashCode_WhenDifferentIds_ShouldHaveDifferentHashCodes()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        // Act
        var hashCode1 = entity1.GetHashCode();
        var hashCode2 = entity2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should work correctly in HashSet")]
    public void GetHashCode_InHashSet_ShouldWorkCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);
        var entity3 = new TestEntity(Guid.NewGuid());

        // Act
        var hashSet = new HashSet<TestEntity> { entity1, entity2, entity3 };

        // Assert
        hashSet.Should().HaveCount(2);
        hashSet.Should().Contain(entity1);
        hashSet.Should().Contain(entity3);
        hashSet.Contains(entity2).Should().BeTrue();
    }

    #endregion

    #region Domain Events Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should start with empty events collection")]
    public void Events_WhenCreated_ShouldBeEmpty()
    {
        // Arrange & Act
        var entity = new TestEntity(Guid.NewGuid());

        // Assert
        entity.Events().Should().BeEmpty();
        entity.Events().Should().NotBeNull();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should add domain event successfully")]
    public void RaiseEvent_WhenValidEvent_ShouldAddEvent()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid());
        var domainEvent = new TestDomainEvent();

        // Act
        entity.RaiseEvent(domainEvent);

        // Assert
        entity.Events().Should().HaveCount(1);
        entity.Events().Should().Contain(domainEvent);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should add multiple domain events successfully")]
    public void RaiseEvent_WhenMultipleEvents_ShouldAddAllEvents()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid());
        var event1 = new TestDomainEvent();
        var event2 = new TestDomainEvent();
        var event3 = new TestDomainEvent();

        // Act
        entity.RaiseEvent(event1);
        entity.RaiseEvent(event2);
        entity.RaiseEvent(event3);

        // Assert
        entity.Events().Should().HaveCount(3);
        entity.Events().Should().Contain([event1, event2, event3]);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should clear events successfully")]
    public void ClearEvents_WhenHasEvents_ShouldClearAllEvents()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid());
        entity.RaiseEvent(new TestDomainEvent());
        entity.RaiseEvent(new TestDomainEvent());

        // Act
        entity.ClearEvents();

        // Assert
        entity.Events().Should().BeEmpty();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return readonly collection of events")]
    public void Events_WhenCalled_ShouldReturnReadOnlyCollection()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid());
        entity.RaiseEvent(new TestDomainEvent());

        // Act
        var events = entity.Events();

        // Assert
        events.Should().BeAssignableTo<IReadOnlyCollection<IDomainEvent>>();
        events.Should().HaveCount(1);
        events.Should().NotBeNull();
    }

    #endregion

    #region Update Entity Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should update UpdatedAt when UpdateEntity is called")]
    public void UpdateEntity_WhenCalled_ShouldSetUpdatedAt()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid());
        var initialUpdatedAt = entity.UpdatedAt;

        // Act
        entity.UpdateEntity();

        // Assert
        entity.UpdatedAt.Should().NotBe(initialUpdatedAt);
        entity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        entity.DeletedAt.Should().BeNull();
    }

    #endregion

    #region Delete Entity Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should set DeletedAt when DeleteEntity is called")]
    public void DeleteEntity_WhenCalled_ShouldSetDeletedAt()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid());
        var initialDeletedAt = entity.DeletedAt;

        // Act
        entity.DeleteEntity();

        // Assert
        entity.DeletedAt.Should().NotBe(initialDeletedAt);
        entity.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    #endregion

    #region Integration Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should work correctly with Dictionary as key")]
    public void BaseEntity_InDictionary_ShouldWorkCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id) { Name = "Entity1" };
        var entity2 = new TestEntity(id) { Name = "Entity2" };
        var entity3 = new TestEntity(Guid.NewGuid()) { Name = "Entity3" };

        var dictionary = new Dictionary<TestEntity, string>
        {
            [entity1] = "Value1", [entity2] = "Value2", [entity3] = "Value3"
        };

        // Assert
        dictionary.Should().HaveCount(2);
        dictionary[entity1].Should().Be("Value2");
        dictionary[entity2].Should().Be("Value2");
        dictionary[entity3].Should().Be("Value3");
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should maintain immutable Id")]
    public void Id_WhenAccessed_ShouldBeImmutable()
    {
        // Arrange
        var originalId = Guid.NewGuid();
        var entity = new TestEntity(originalId);

        // Act & Assert
        entity.Id.Should().Be(originalId);
    }

    #endregion
}