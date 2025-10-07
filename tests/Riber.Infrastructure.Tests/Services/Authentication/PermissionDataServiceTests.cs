using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Riber.Application.Exceptions;
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

    [Fact(DisplayName = "Validating permission when permission exists and is active should return true")]
    public async Task ValidateAsync_WhenPermissionExistsAndIsActive_ShouldReturnTrue()
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

        _mockMemoryCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny))
            .Returns(false);

        var mockCacheEntry = new Mock<ICacheEntry>();
        _mockMemoryCache
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(mockCacheEntry.Object);

        // Act
        var result = await _permissionDataService.ValidateAsync(permission.Name);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Validating permission when permission exists and is inactive should return false")]
    public async Task ValidateAsync_WhenPermissionExistsAndIsInactive_ShouldReturnFalse()
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

        _mockMemoryCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny))
            .Returns(false);

        var mockCacheEntry = new Mock<ICacheEntry>();
        _mockMemoryCache
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(mockCacheEntry.Object);

        // Act
        var result = await _permissionDataService.ValidateAsync(permission.Name);

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Validating permission when permission does not exist should throw NotFoundException")]
    public async Task ValidateAsync_WhenPermissionDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var permissionName = "create-test";
        var permission = CreateFaker<ApplicationPermission>()
            .RuleFor(x => x.Id, f => f.Random.UInt())
            .RuleFor(x => x.Name, f => f.Random.String2(10))
            .RuleFor(x => x.Description, f => f.Random.String2(10))
            .RuleFor(x => x.IsActive, false)
            .RuleFor(x => x.Category, "test")
            .Generate(10);

        _mockMemoryCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny))
            .Returns((object _, out object? value) =>
            {
                value = permission.AsEnumerable();
                return true;
            });

        // Act
        var result = async () => await _permissionDataService.ValidateAsync(permissionName);

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage(NotFoundErrors.Permission);
    }

    [Fact(DisplayName = "Updating permission status when permission exists should toggle IsActive")]
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
        _mockMemoryCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny))
            .Returns(false);

        var mockCacheEntry = new Mock<ICacheEntry>();
        _mockMemoryCache
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(mockCacheEntry.Object);

        _mockMemoryCache.Setup(x => x.Remove(It.IsAny<object>()));

        // Act
        await _permissionDataService.UpdatePermissionStatusAsync(permission.Name);

        // Assert
        var updatedPermission = await _context.Set<ApplicationPermission>()
            .FirstAsync(p => p.Name == permission.Name);
        updatedPermission.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = "Updating permission status when permission does not exist should throw NotFoundException")]
    public async Task UpdatePermissionStatusAsync_WhenPermissionDoesNotExist_ShouldThrowNotFoundException()
    {
        var permissionName = "create-test";
        var permission = CreateFaker<ApplicationPermission>()
            .RuleFor(x => x.Id, f => f.Random.UInt())
            .RuleFor(x => x.Name, f => f.Random.String2(10))
            .RuleFor(x => x.Description, f => f.Random.String2(10))
            .RuleFor(x => x.IsActive, false)
            .RuleFor(x => x.Category, "test")
            .Generate(10);

        await _context.Set<ApplicationPermission>().AddRangeAsync(permission);
        await _context.SaveChangesAsync();

        _mockMemoryCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny))
            .Returns(false);

        var mockCacheEntry = new Mock<ICacheEntry>();
        _mockMemoryCache
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(mockCacheEntry.Object);

        // Act
        var result = async () => await _permissionDataService.UpdatePermissionStatusAsync(permissionName);

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage(NotFoundErrors.Permission);
    }

    [Fact(DisplayName = "Getting all permissions should return mapped PermissionDTO collection")]
    public async Task GetAllWithDescriptionsAsync_WhenCalled_ShouldReturnMappedPermissionDTOCollection()
    {
        // Arrange
        var permissions = CreateFaker<ApplicationPermission>()
            .RuleFor(x => x.Id, f => f.Random.UInt())
            .RuleFor(x => x.Name, f => f.Random.String2(10))
            .RuleFor(x => x.Description, f => f.Random.String2(10))
            .RuleFor(x => x.IsActive, false)
            .RuleFor(x => x.Category, "test")
            .Generate(10);

        _context.Set<ApplicationPermission>().AddRange(permissions);
        await _context.SaveChangesAsync();

        _mockMemoryCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny))
            .Returns((object? _, out object? value) =>
            {
                value = permissions.AsEnumerable();
                return true;
            });

        var mockCacheEntry = new Mock<ICacheEntry>();
        _mockMemoryCache
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(mockCacheEntry.Object);

        // Act
        var result = await _permissionDataService.GetAllWithDescriptionsAsync();

        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(permissions.Count);
        result.Should().OnlyContain(x => !string.IsNullOrEmpty(x.Description));
    }
}