using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SnackFlow.Domain.Tests;
using SnackFlow.Infrastructure.Services.Authentication;

namespace SnackFlow.Infrastructure.Tests.Services;

public sealed class CurrentUserServiceUnitTests : BaseTest
{
    #region Setup

    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly CurrentUserService _currentUserService;

    public CurrentUserServiceUnitTests()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _currentUserService = new CurrentUserService(
            _mockHttpContextAccessor.Object
        );
    }
    
    #endregion

    [Fact(DisplayName = "Should return empty array when user is null")]
    public void GetPermissions_WhenUserIsNull_ShouldReturnEmptyArray()
    {
        // Arrange
        _mockHttpContextAccessor.Setup(x => x.HttpContext)
            .Returns((HttpContext?)null);
        
        // Act
        var result = _currentUserService.GetPermissions();
        
        // Assert
        result.Should().BeEmpty();
    }

    [Fact(DisplayName = "Should return empty array when user has no permissions")]
    public void GetPermissions_WhenUserHasNoPermissions_ShouldReturnEmptyArray()
    {
        // Arrange
        Claim[] claims = [
            new(ClaimTypes.Name, Guid.CreateVersion7().ToString()),
            new(ClaimTypes.Email, _faker.Person.Email)
        ];
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        
        _mockHttpContextAccessor.Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext { User = claimsPrincipal });
        
        // Act
        var result = _currentUserService.GetPermissions();
        
        // Assert
        result.Should().BeEmpty();
    }

    [Fact(DisplayName = "Should return permission array when user has permissions")]
    public void GetPermissions_WhenUserHasPermissions_ShouldReturnPermissionArray()
    {
        // Arrange
        string[] permissions = ["read", "write"];
        Claim[] claims = [
            new(ClaimTypes.Name, Guid.CreateVersion7().ToString()),
            new(ClaimTypes.Email, _faker.Person.Email),
            new("permission", permissions[0]),
            new("permission", permissions[1])
        ];
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
        
        _mockHttpContextAccessor.Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext { User = claimsPrincipal });
        
        // Act
        var result = _currentUserService.GetPermissions();
        
        // Assert
        result.Should().BeEquivalentTo(permissions);
    }

    [Fact(DisplayName = "Should return null when user is null")]
    public void GetUserId_WhenUserIsNull_ShouldReturnNull()
    {
        // Arrange
        _mockHttpContextAccessor.Setup(x => x.HttpContext)
            .Returns((HttpContext?)null);
        
        // Act
        var result = _currentUserService.GetUserId();
        
        // Assert
        result.Should().BeNull();
    }

    [Fact(DisplayName = "Should return null when user has no name identifier")]
    public void GetUserId_WhenUserHasNoNameIdentifier_ShouldReturnNull()
    {
        // Arrange
        Claim[] claims = [new(ClaimTypes.Email, _faker.Person.Email)];
        
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        
        _mockHttpContextAccessor.Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext { User = claimsPrincipal });
        
        // Act
        var result = _currentUserService.GetUserId();
        
        // Assert
        result.Should().BeNull();
    }

    [Fact(DisplayName = "Should return null when user has invalid guid")]
    public void GetUserId_WhenUserHasInvalidGuid_ShouldReturnNull()
    {
        // Arrange
        Claim[] claims = [
            new(ClaimTypes.NameIdentifier, "this-is-definitely-not-a-valid-guid-string-at-all"),
            new(ClaimTypes.Email, _faker.Person.Email)
        ];
        
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        
        _mockHttpContextAccessor.Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext { User = claimsPrincipal });
        
        // Act
        var result = _currentUserService.GetUserId();
        
        // Assert
        result.Should().BeNull();
    }

    [Fact(DisplayName = "Should return user id when user has valid guid")]
    public void GetUserId_WhenUserHasValidGuid_ShouldReturnUserId()
    {
        // Arrange
        var guid = Guid.CreateVersion7();
        
        Claim[] claims = [
            new(ClaimTypes.NameIdentifier, guid.ToString()),
            new(ClaimTypes.Email, _faker.Person.Email)
        ];
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
        
        _mockHttpContextAccessor.Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext { User = claimsPrincipal });
        
        // Act
        var result = _currentUserService.GetUserId();
        
        // Assert
        result.Should().Be(guid);
    }
}