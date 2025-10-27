using System.Net;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Tests;
using Riber.Infrastructure.Persistence;
using Riber.Infrastructure.Persistence.Identity;
using Riber.Infrastructure.Services.Authentication;

namespace Riber.Infrastructure.Tests.Services.Authentication;

public sealed class PermissionDataServiceTests : BaseTest
{
    #region Fields and Setup

    private readonly AppDbContext _context;
    private readonly Mock<IMemoryCache> _mockMemoryCache;
    private readonly PermissionDataService _permissionDataService;

    public PermissionDataServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _mockMemoryCache = new Mock<IMemoryCache>();

        _permissionDataService = new PermissionDataService(
            _context,
            _mockMemoryCache.Object
        );
    }

    #endregion

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return success when permission exists and is active")]
    public async Task ValidateAsync_WhenPermissionExistsAndIsActive_ShouldReturnSuccess()
    {
        // Arrange
        var permission = CreateFaker<ApplicationPermission>()
            .RuleFor(x => x.Id, f => f.Random.UInt())
            .RuleFor(x => x.Name, "create-test")
            .RuleFor(x => x.Description, f => f.Random.String2(10))
            .RuleFor(x => x.IsActive, true)
            .RuleFor(x => x.Category, "test")
            .Generate();

        _context.Set<ApplicationPermission>().Add(permission);
        await _context.SaveChangesAsync();

        SetupCacheMiss();

        // Act
        var result = await _permissionDataService.ValidateAsync(permission.Name);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return failure when permission exists but is inactive")]
    public async Task ValidateAsync_WhenPermissionExistsAndIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var permission = CreateFaker<ApplicationPermission>()
            .RuleFor(x => x.Id, f => f.Random.UInt())
            .RuleFor(x => x.Name, "create-test")
            .RuleFor(x => x.Description, f => f.Random.String2(10))
            .RuleFor(x => x.IsActive, false)
            .RuleFor(x => x.Category, "test")
            .Generate();

        _context.Set<ApplicationPermission>().Add(permission);
        await _context.SaveChangesAsync();

        SetupCacheMiss();

        // Act
        var result = await _permissionDataService.ValidateAsync(permission.Name);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        result.Error.Message.Should().Be("Permissão está inativa");
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return not found when permission does not exist")]
    public async Task ValidateAsync_WhenPermissionDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var permissionName = "create-test";
        var permissions = CreateFaker<ApplicationPermission>()
            .RuleFor(x => x.Id, f => f.Random.UInt())
            .RuleFor(x => x.Name, f => f.Random.String2(10))
            .RuleFor(x => x.Description, f => f.Random.String2(10))
            .RuleFor(x => x.IsActive, false)
            .RuleFor(x => x.Category, "test")
            .Generate(10);

        SetupCacheHit(permissions);

        // Act
        var result = await _permissionDataService.ValidateAsync(permissionName);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Error.Message.Should().Be(NotFoundErrors.Permission);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should toggle permission status successfully")]
    public async Task UpdatePermissionStatusAsync_WhenPermissionExists_ShouldToggleIsActive()
    {
        // Arrange
        var permission = CreateFaker<ApplicationPermission>()
            .RuleFor(x => x.Id, f => f.Random.UInt())
            .RuleFor(x => x.Name, "create-test")
            .RuleFor(x => x.Description, f => f.Random.String2(10))
            .RuleFor(x => x.IsActive, false)
            .RuleFor(x => x.Category, "test")
            .Generate();

        _context.Set<ApplicationPermission>().Add(permission);
        await _context.SaveChangesAsync();
        _context.Entry(permission).State = EntityState.Detached;

        SetupCacheMiss();
        _mockMemoryCache.Setup(x => x.Remove(It.IsAny<object>()));

        // Act
        var result = await _permissionDataService.UpdatePermissionStatusAsync(permission.Name);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var updatedPermission = await _context.Set<ApplicationPermission>()
            .FirstAsync(p => p.Name == permission.Name);
        updatedPermission.IsActive.Should().BeTrue();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return not found when updating non-existent permission")]
    public async Task UpdatePermissionStatusAsync_WhenPermissionDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var permissionName = "create-test";
        var permissions = CreateFaker<ApplicationPermission>()
            .RuleFor(x => x.Id, f => f.Random.UInt())
            .RuleFor(x => x.Name, f => f.Random.String2(10))
            .RuleFor(x => x.Description, f => f.Random.String2(10))
            .RuleFor(x => x.IsActive, false)
            .RuleFor(x => x.Category, "test")
            .Generate(10);

        await _context.Set<ApplicationPermission>().AddRangeAsync(permissions);
        await _context.SaveChangesAsync();

        SetupCacheMiss();

        // Act
        var result = await _permissionDataService.UpdatePermissionStatusAsync(permissionName);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Error.Message.Should().Be(NotFoundErrors.Permission);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return all permissions with descriptions")]
    public async Task GetAllWithDescriptionsAsync_WhenCalled_ShouldReturnAllPermissions()
    {
        // Arrange
        var permissions = CreateFaker<ApplicationPermission>()
            .RuleFor(x => x.Id, f => f.Random.UInt())
            .RuleFor(x => x.Name, f => f.Random.String2(10))
            .RuleFor(x => x.Description, f => f.Random.String2(10))
            .RuleFor(x => x.IsActive, f => f.Random.Bool())
            .RuleFor(x => x.Category, "test")
            .Generate(10);

        SetupCacheHit(permissions);

        // Act
        var result = await _permissionDataService.GetAllWithDescriptionsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrEmpty();
        result.Value.Should().HaveCount(permissions.Count);
        result.Value.Should().OnlyContain(x => !string.IsNullOrEmpty(x.Description));
    }

    #region Helper Methods

    private void SetupCacheMiss()
    {
        _mockMemoryCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny))
            .Returns(false);

        var mockCacheEntry = new Mock<ICacheEntry>();
        _mockMemoryCache
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(mockCacheEntry.Object);
    }

    private void SetupCacheHit(IEnumerable<ApplicationPermission> permissions)
    {
        _mockMemoryCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny))
            .Returns((object _, out object? value) =>
            {
                value = permissions;
                return true;
            });
    }

    #endregion
}