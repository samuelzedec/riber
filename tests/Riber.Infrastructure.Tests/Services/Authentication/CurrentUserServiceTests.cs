using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Riber.Domain.Tests;
using Riber.Infrastructure.Services.Authentication;

namespace Riber.Infrastructure.Tests.Services.Authentication;

public sealed class CurrentUserServiceTests : BaseTest
{
    #region Setup

    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly CurrentUserService _currentUserService;

    public CurrentUserServiceTests()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _currentUserService = new CurrentUserService(
            _mockHttpContextAccessor.Object
        );
    }

    #endregion

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should throw exception when user id claim is null")]
    public void GetUserId_WhenUserIdClaimIsNull_ShouldThrowException()
    {
        // Arrange
        Claim[] claims =
        [
            new(ClaimTypes.Email, _faker.Person.Email)
            // Sem ClaimTypes.NameIdentifier
        ];
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));

        _mockHttpContextAccessor.Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext { User = claimsPrincipal });

        // Act & Assert
        var act = () => _currentUserService.GetUserId();
        act.Should().Throw<NullReferenceException>();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should throw exception when user id is invalid guid")]
    public void GetUserId_WhenUserIdIsInvalidGuid_ShouldThrowException()
    {
        // Arrange
        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, "invalid-guid"),
            new(ClaimTypes.Email, _faker.Person.Email)
        ];
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));

        _mockHttpContextAccessor.Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext { User = claimsPrincipal });

        // Act & Assert
        var act = () => _currentUserService.GetUserId();
        act.Should().Throw<FormatException>();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return company id when user has valid company guid")]
    public void GetCompanyId_WhenUserHasValidCompanyGuid_ShouldReturnCompanyId()
    {
        // Arrange
        var companyId = Guid.CreateVersion7();

        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, Guid.CreateVersion7().ToString()),
            new("companyId", companyId.ToString()),
            new(ClaimTypes.Email, _faker.Person.Email)
        ];
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));

        _mockHttpContextAccessor.Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext { User = claimsPrincipal });

        // Act
        var result = _currentUserService.GetCompanyId();

        // Assert
        result.Should().Be(companyId);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should throw exception when company id claim is null")]
    public void GetCompanyId_WhenCompanyIdClaimIsNull_ShouldThrowException()
    {
        // Arrange
        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, Guid.CreateVersion7().ToString()),
            new(ClaimTypes.Email, _faker.Person.Email)
            // Sem "companyId"
        ];
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));

        _mockHttpContextAccessor.Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext { User = claimsPrincipal });

        // Act & Assert
        var act = () => _currentUserService.GetCompanyId();
        act.Should().Throw<NullReferenceException>();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return Guid.Empty when company id is invalid guid")]
    public void GetCompanyId_WhenCompanyIdIsInvalidGuid_ShouldReturnGuidEmpty()
    {
        // Arrange
        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, Guid.CreateVersion7().ToString()),
            new("companyId", "invalid-guid"),
            new(ClaimTypes.Email, _faker.Person.Email)
        ];
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));

        _mockHttpContextAccessor.Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext { User = claimsPrincipal });

        // Act
        var result = _currentUserService.GetCompanyId();

        // Assert
        result.Should().Be(Guid.Empty);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return Guid.Empty when company id is string empty")]
    public void GetCompanyId_WhenCompanyIdIStringEmpty_ShouldReturnGuidEmpty()
    {
        // Arrange
        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, Guid.CreateVersion7().ToString()),
            new("companyId", ""),
            new(ClaimTypes.Email, _faker.Person.Email)
        ];
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));

        _mockHttpContextAccessor.Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext { User = claimsPrincipal });

        // Act
        var result = _currentUserService.GetCompanyId();

        // Assert
        result.Should().Be(Guid.Empty);
    }
}