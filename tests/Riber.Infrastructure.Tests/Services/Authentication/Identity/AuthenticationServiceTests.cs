using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Riber.Api.Tests;
using Riber.Api.Tests.Fixtures;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Tests.Services.Authentication.Identity;

public sealed class AuthenticationServiceTests : IntegrationTestBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthenticationServiceTests(WebAppFixture webAppFixture, DatabaseFixture databaseFixture)
        : base(webAppFixture, databaseFixture)
    {
        var scope = webAppFixture.GetFactory().Services.CreateScope();
        _authenticationService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    }

    #region LoginAsync Tests

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should login successfully with valid username and password")]
    public async Task LoginAsync_WithValidUsernameAndPassword_ShouldReturnUserDetails()
    {
        // Arrange
        const string userName = "admin123";
        const string password = "Admin@123";

        // Act
        var result = await _authenticationService.LoginAsync(userName, password);

        // Assert
        result.Should().NotBeNull();
        result.UserName.Should().Be(userName);
        result.Email.Should().NotBeNullOrEmpty();
        result.SecurityStamp.Should().NotBeNullOrEmpty();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should login successfully with valid email and password")]
    public async Task LoginAsync_WithValidEmailAndPassword_ShouldReturnUserDetails()
    {
        // Arrange
        const string email = "admin@user.com";
        const string password = "Admin@123";

        // Act
        var result = await _authenticationService.LoginAsync(email, password);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(email);
        result.UserName.Should().NotBeNullOrEmpty();
        result.SecurityStamp.Should().NotBeNullOrEmpty();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should return null when user does not exist")]
    public async Task LoginAsync_WithNonExistentUser_ShouldReturnNull()
    {
        // Arrange
        const string userName = "nonexistentuser";
        const string password = "SomePassword@123";

        // Act
        var result = await _authenticationService.LoginAsync(userName, password);

        // Assert
        result.Should().BeNull();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should return null when password is incorrect")]
    public async Task LoginAsync_WithIncorrectPassword_ShouldReturnNull()
    {
        // Arrange
        const string userName = "admin123";
        const string password = "WrongPassword@123";

        // Act
        var result = await _authenticationService.LoginAsync(userName, password);

        // Assert
        result.Should().BeNull();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should update security stamp on successful login")]
    public async Task LoginAsync_OnSuccess_ShouldUpdateSecurityStamp()
    {
        // Arrange
        const string userName = "admin123";
        const string password = "Admin@123";

        var userBefore = await _userManager.FindByNameAsync(userName);
        var securityStampBefore = userBefore!.SecurityStamp;

        // Act
        var result = await _authenticationService.LoginAsync(userName, password);

        // Assert
        result.Should().NotBeNull();

        var userAfter = await _userManager.FindByNameAsync(userName);
        userAfter!.SecurityStamp.Should().NotBe(securityStampBefore);
        result.SecurityStamp.Should().Be(userAfter.SecurityStamp);
    }

    [Trait("Category", "Integration")]
    [Theory(DisplayName = "Should handle case-insensitive username and email")]
    [InlineData("ADMIN123", "Admin@123")]
    [InlineData("Admin123", "Admin@123")]
    [InlineData("ADMIN@USER.COM", "Admin@123")]
    [InlineData("Admin@User.Com", "Admin@123")]
    public async Task LoginAsync_WithDifferentCasing_ShouldReturnUserDetails(string userNameOrEmail, string password)
    {
        // Act
        var result = await _authenticationService.LoginAsync(userNameOrEmail, password);

        // Assert
        result.Should().NotBeNull();
        result.UserName.Should().NotBeNullOrEmpty();
        result.Email.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region RefreshSecurityStampAsync Tests

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should refresh security stamp for valid user id")]
    public async Task RefreshSecurityStampAsync_WithValidUserId_ShouldReturnTrue()
    {
        // Arrange
        const string userName = "admin123";
        var user = await _userManager.FindByNameAsync(userName);
        var securityStampBefore = user!.SecurityStamp;

        // Act
        var result = await _authenticationService.RefreshSecurityStampAsync(user.Id.ToString());

        // Assert
        result.Should().BeTrue();

        var userAfter = await _userManager.FindByIdAsync(user.Id.ToString());
        userAfter!.SecurityStamp.Should().NotBe(securityStampBefore);
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should return false when user id does not exist")]
    public async Task RefreshSecurityStampAsync_WithNonExistentUserId_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentUserId = Guid.CreateVersion7().ToString();

        // Act
        var result = await _authenticationService.RefreshSecurityStampAsync(nonExistentUserId);

        // Assert
        result.Should().BeFalse();
    }

    #endregion
}