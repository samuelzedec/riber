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

public sealed class PermissionDataServiceTests(WebAppFixture webAppFixture, DatabaseFixture databaseFixture)
    : IntegrationTestBase(webAppFixture, databaseFixture)
{
    private readonly Mock<IMemoryCache> _mockMemoryCache = new();

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should return success when permission exists and is active")]
    public async Task ValidateAsync_WhenPermissionExistsAndIsActive_ShouldReturnSuccess()
    {
        // Arrange
        var context = GetDbContext();
        var permissionService = CreatePermissionService(context);
        
        var faker = new Faker();
        var permission = new ApplicationPermission
        {
            Id = faker.Random.UInt(),
            Name = "create-test-active",
            Description = faker.Random.String2(10),
            IsActive = true,
            Category = "test"
        };

        context.Set<ApplicationPermission>().Add(permission);
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
        
        var faker = new Faker();
        var permission = new ApplicationPermission
        {
            Id = faker.Random.UInt(),
            Name = "create-test-inactive",
            Description = faker.Random.String2(10),
            IsActive = false,
            Category = "test"
        };

        context.Set<ApplicationPermission>().Add(permission);
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
        
        var faker = new Faker();
        var permissionName = "create-test-nonexistent";
        var permissions = Enumerable.Range(1, 10).Select(_ => new ApplicationPermission
        {
            Id = faker.Random.UInt(),
            Name = faker.Random.String2(10),
            Description = faker.Random.String2(10),
            IsActive = false,
            Category = "test"
        }).ToList();

        SetupCacheHit(permissions);

        // Act
        var result = await permissionService.ValidateAsync(permissionName);

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
        
        var faker = new Faker();
        var permission = new ApplicationPermission
        {
            Id = faker.Random.UInt(),
            Name = "create-test-toggle",
            Description = faker.Random.String2(10),
            IsActive = false,
            Category = "test"
        };

        context.Set<ApplicationPermission>().Add(permission);
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
        
        var faker = new Faker();
        var permissionName = "create-test-update-nonexistent";
        var permissions = Enumerable.Range(1, 10).Select(_ => new ApplicationPermission
        {
            Id = faker.Random.UInt(),
            Name = faker.Random.String2(10),
            Description = faker.Random.String2(10),
            IsActive = false,
            Category = "test"
        }).ToList();

        await context.Set<ApplicationPermission>().AddRangeAsync(permissions);
        await context.SaveChangesAsync();

        SetupCacheMiss();

        // Act
        var result = await permissionService.UpdatePermissionStatusAsync(permissionName);

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
        
        var faker = new Faker();
        var permissions = Enumerable.Range(1, 10).Select(i => new ApplicationPermission
        {
            Id = faker.Random.UInt(),
            Name = $"permission-{i}",
            Description = faker.Random.String2(10),
            IsActive = faker.Random.Bool(),
            Category = "test"
        }).ToList();

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
    {
        return new Infrastructure.Services.Authentication.PermissionDataService(
            context,
            _mockMemoryCache.Object
        );
    }

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