using System.Net;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Domain.Constants.Messages.Common;
using Riber.Infrastructure.Persistence;
using Riber.Infrastructure.Persistence.Identity;
using Riber.Api.Tests;
using Riber.Api.Tests.Fixtures;

namespace Riber.Infrastructure.Tests.Services.Authentication;

public sealed class PermissionDataServiceTests(
    WebAppFixture webAppFixture, 
    DatabaseFixture databaseFixture)
    : IntegrationTestBase(webAppFixture, databaseFixture)
{
    private readonly Mock<IMemoryCache> _mockMemoryCache = new();
    private readonly Faker _faker = new();

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should return success when permission exists and is active")]
    public async Task ValidateAsync_WhenPermissionExistsAndIsActive_ShouldReturnSuccess()
    {
        // Arrange
        var context = GetDbContext();
        var permissionService = CreatePermissionService(context);
        var permission = CreatePermission("create-test-active", isActive: true);

        await context.Set<ApplicationPermission>().AddAsync(permission);
        await context.SaveChangesAsync();
        SetupCacheMiss();

        // Act
        var result = await permissionService.ValidateAsync(permission.Name);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should return failure when permission exists but is inactive")]
    public async Task ValidateAsync_WhenPermissionExistsAndIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var context = GetDbContext();
        var permissionService = CreatePermissionService(context);
        var permission = CreatePermission("create-test-inactive", isActive: false);

        await context.Set<ApplicationPermission>().AddAsync(permission);
        await context.SaveChangesAsync();
        SetupCacheMiss();

        // Act
        var result = await permissionService.ValidateAsync(permission.Name);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        result.Error.Message.Should().Be("Permissão está inativa");
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should return not found when permission does not exist")]
    public async Task ValidateAsync_WhenPermissionDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var context = GetDbContext();
        var permissionService = CreatePermissionService(context);
        var permissions = CreatePermissions(count: 10);

        SetupCacheHit(permissions);

        // Act
        var result = await permissionService.ValidateAsync("create-test-nonexistent");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Error.Message.Should().Be(NotFoundErrors.Permission);
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should toggle permission status successfully")]
    public async Task UpdatePermissionStatusAsync_WhenPermissionExists_ShouldToggleIsActive()
    {
        // Arrange
        var context = GetDbContext();
        var permissionService = CreatePermissionService(context);
        var permission = CreatePermission("create-test-toggle", isActive: false);

        await context.Set<ApplicationPermission>().AddAsync(permission);
        await context.SaveChangesAsync();
        context.Entry(permission).State = EntityState.Detached;

        SetupCacheMiss();
        _mockMemoryCache.Setup(x => x.Remove(It.IsAny<object>()));

        // Act
        var result = await permissionService.UpdatePermissionStatusAsync(permission.Name);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var updatedPermission = await context.Set<ApplicationPermission>()
            .FirstAsync(p => p.Name == permission.Name);
        updatedPermission.IsActive.Should().BeTrue();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should return not found when updating non-existent permission")]
    public async Task UpdatePermissionStatusAsync_WhenPermissionDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var context = GetDbContext();
        var permissionService = CreatePermissionService(context);
        var permissions = CreatePermissions(count: 10);

        await context.Set<ApplicationPermission>().AddRangeAsync(permissions);
        await context.SaveChangesAsync();
        SetupCacheMiss();

        // Act
        var result = await permissionService.UpdatePermissionStatusAsync("create-test-update-nonexistent");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Error.Message.Should().Be(NotFoundErrors.Permission);
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should return all permissions with descriptions")]
    public async Task GetAllWithDescriptionsAsync_WhenCalled_ShouldReturnAllPermissions()
    {
        // Arrange
        var context = GetDbContext();
        var permissionService = CreatePermissionService(context);
        var permissions = CreatePermissions(count: 10);

        SetupCacheHit(permissions);

        // Act
        var result = await permissionService.GetAllWithDescriptionsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrEmpty();
        result.Value.Should().HaveCount(permissions.Count);
        result.Value.Should().OnlyContain(x => !string.IsNullOrEmpty(x.Description));
    }

    #region Helper Methods

    private IPermissionDataService CreatePermissionService(AppDbContext context)
        => new Infrastructure.Services.Authentication.PermissionDataService(context, _mockMemoryCache.Object);

    private ApplicationPermission CreatePermission(string name, bool isActive = true)
        => new()
        {
            Id = _faker.Random.UInt(),
            Name = name,
            Description = _faker.Random.String2(10),
            IsActive = isActive,
            Category = "test"
        };

    private List<ApplicationPermission> CreatePermissions(int count)
        => Enumerable.Range(1, count)
            .Select(i => new ApplicationPermission
            {
                Id = _faker.Random.UInt(),
                Name = $"permission-{i}",
                Description = _faker.Random.String2(10),
                IsActive = _faker.Random.Bool(),
                Category = "test"
            })
            .ToList();

    private void SetupCacheMiss()
    {
        _mockMemoryCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny))
            .Returns(false);

        _mockMemoryCache
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(new Mock<ICacheEntry>().Object);
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