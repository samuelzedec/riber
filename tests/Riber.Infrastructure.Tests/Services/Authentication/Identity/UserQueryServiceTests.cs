using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Riber.Api.Tests;
using Riber.Api.Tests.Fixtures;
using Riber.Application.Abstractions.Services.Authentication;

namespace Riber.Infrastructure.Tests.Services.Authentication.Identity;

public sealed class UserQueryServiceTests : IntegrationTestBase
{
    private readonly IUserQueryService _userQueryService;

    public UserQueryServiceTests(WebAppFixture webAppFixture, DatabaseFixture databaseFixture) 
        : base(webAppFixture, databaseFixture)
    {
        var scope = webAppFixture.GetFactory().Services.CreateScope();
        _userQueryService = scope.ServiceProvider.GetRequiredService<IUserQueryService>();
    }

    #region FindByIdAsync Tests

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should find user by valid id")]
    public async Task FindByIdAsync_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var userId = Guid.Parse("e6fba186-c1e7-4083-90a6-966c421720e5");

        // Act
        var result = await _userQueryService.FindByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.UserName.Should().NotBeNullOrEmpty();
        result.Email.Should().NotBeNullOrEmpty();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should return null when user id does not exist")]
    public async Task FindByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _userQueryService.FindByIdAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should include user domain in result")]
    public async Task FindByIdAsync_ShouldIncludeUserDomain()
    {
        // Arrange
        var userId = Guid.Parse("e6fba186-c1e7-4083-90a6-966c421720e5");

        // Act
        var result = await _userQueryService.FindByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.UserDomainId.Should().NotBeEmpty();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should include roles in result")]
    public async Task FindByIdAsync_ShouldIncludeRoles()
    {
        // Arrange
        var userId = Guid.Parse("e6fba186-c1e7-4083-90a6-966c421720e5");

        // Act
        var result = await _userQueryService.FindByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.Roles.Should().NotBeEmpty();
    }

    #endregion

    #region FindByEmailAsync Tests

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should find user by valid email")]
    public async Task FindByEmailAsync_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        const string email = "admin@user.com";

        // Act
        var result = await _userQueryService.FindByEmailAsync(email);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
        result.UserName.Should().NotBeNullOrEmpty();
        result.Id.Should().NotBeEmpty();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should find user by email case insensitive")]
    public async Task FindByEmailAsync_WithDifferentCase_ShouldReturnUser()
    {
        // Arrange
        const string email = "ADMIN@USER.COM";

        // Act
        var result = await _userQueryService.FindByEmailAsync(email);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("admin@user.com");
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should return null when email does not exist")]
    public async Task FindByEmailAsync_WithNonExistentEmail_ShouldReturnNull()
    {
        // Arrange
        const string email = "nonexistent@example.com";

        // Act
        var result = await _userQueryService.FindByEmailAsync(email);

        // Assert
        result.Should().BeNull();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should include user domain when finding by email")]
    public async Task FindByEmailAsync_ShouldIncludeUserDomain()
    {
        // Arrange
        const string email = "admin@user.com";

        // Act
        var result = await _userQueryService.FindByEmailAsync(email);

        // Assert
        result.Should().NotBeNull();
        result!.UserDomainId.Should().NotBeEmpty();
    }

    #endregion

    #region FindByUserNameAsync Tests

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should find user by valid username")]
    public async Task FindByUserNameAsync_WithValidUserName_ShouldReturnUser()
    {
        // Arrange
        const string userName = "admin123";

        // Act
        var result = await _userQueryService.FindByUserNameAsync(userName);

        // Assert
        result.Should().NotBeNull();
        result!.UserName.Should().Be(userName);
        result.Email.Should().NotBeNullOrEmpty();
        result.Id.Should().NotBeEmpty();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should find user by username case insensitive")]
    public async Task FindByUserNameAsync_WithDifferentCase_ShouldReturnUser()
    {
        // Arrange
        const string userName = "ADMIN123";

        // Act
        var result = await _userQueryService.FindByUserNameAsync(userName);

        // Assert
        result.Should().NotBeNull();
        result!.UserName.Should().Be("admin123");
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should return null when username does not exist")]
    public async Task FindByUserNameAsync_WithNonExistentUserName_ShouldReturnNull()
    {
        // Arrange
        const string userName = "nonexistentuser";

        // Act
        var result = await _userQueryService.FindByUserNameAsync(userName);

        // Assert
        result.Should().BeNull();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should include roles when finding by username")]
    public async Task FindByUserNameAsync_ShouldIncludeRoles()
    {
        // Arrange
        const string userName = "admin123";

        // Act
        var result = await _userQueryService.FindByUserNameAsync(userName);

        // Assert
        result.Should().NotBeNull();
        result!.Roles.Should().NotBeEmpty();
    }

    #endregion

    #region FindByPhoneAsync Tests

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should find user by valid phone number")]
    public async Task FindByPhoneAsync_WithValidPhoneNumber_ShouldReturnUser()
    {
        // Arrange
        const string phoneNumber = "92998648878";

        // Act
        var result = await _userQueryService.FindByPhoneAsync(phoneNumber);

        // Assert
        result.Should().NotBeNull();
        result!.PhoneNumber.Should().Be(phoneNumber);
        result.UserName.Should().NotBeNullOrEmpty();
        result.Email.Should().NotBeNullOrEmpty();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should return null when phone number does not exist")]
    public async Task FindByPhoneAsync_WithNonExistentPhoneNumber_ShouldReturnNull()
    {
        // Arrange
        const string phoneNumber = "(11) 11111-1111";

        // Act
        var result = await _userQueryService.FindByPhoneAsync(phoneNumber);

        // Assert
        result.Should().BeNull();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should be case sensitive for phone number")]
    public async Task FindByPhoneAsync_IsCaseSensitive()
    {
        // Arrange
        const string phoneNumber = "92998648878";

        // Act
        var result = await _userQueryService.FindByPhoneAsync(phoneNumber);

        // Assert
        result.Should().NotBeNull();
        result.PhoneNumber.Should().Be(phoneNumber);
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should include user domain when finding by phone")]
    public async Task FindByPhoneAsync_ShouldIncludeUserDomain()
    {
        // Arrange
        const string phoneNumber = "92998648878";

        // Act
        var result = await _userQueryService.FindByPhoneAsync(phoneNumber);

        // Assert
        result.Should().NotBeNull();
        result.UserDomainId.Should().NotBeEmpty();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should include roles when finding by phone")]
    public async Task FindByPhoneAsync_ShouldIncludeRoles()
    {
        // Arrange
        const string phoneNumber = "92998648878";

        // Act
        var result = await _userQueryService.FindByPhoneAsync(phoneNumber);

        // Assert
        result.Should().NotBeNull();
        result.Roles.Should().NotBeEmpty();
    }

    #endregion
}