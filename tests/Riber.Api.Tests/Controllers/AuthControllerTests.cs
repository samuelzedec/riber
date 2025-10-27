using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Riber.Api.Tests.Fixtures;
using Riber.Application.Features.Auths.Commands.Login;
using Riber.Application.Features.Auths.Queries.GetPermissions;
using Riber.Application.Features.Auths.Queries.GetRefreshToken;
using Riber.Domain.Constants.Messages.Common;
using Xunit;

namespace Riber.Api.Tests.Controllers;

public sealed class AuthControllerTests(WebAppFixture webAppFixture, DatabaseFixture databaseFixture)
    : IntegrationTestBase(webAppFixture, databaseFixture)
{
    #region Login Tests

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should login successfully with valid credentials")]
    public async Task Should_LoginSuccessfully_WithValidCredentials()
    {
        // Arrange
        var credentials = new LoginCommand("admin123", "Admin@123");

        // Act
        var response = await Client.PostAsJsonAsync("api/v1/auth/login", credentials);

        // Assert
        var loginResponse = await ReadResultValueAsync<LoginCommandResponse>(response);
        loginResponse.Should().NotBeNull();
        loginResponse.IsSuccess.Should().BeTrue();
        loginResponse.Value.Should().NotBeNull();
        loginResponse.Value!.Token.Should().NotBeNullOrEmpty();
        loginResponse.Value.RefreshToken.Should().NotBeNullOrEmpty();
        loginResponse.Value.UserDomainId.Should().NotBeEmpty();
        loginResponse.Value.UserApplicationId.Should().NotBeEmpty();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should not login successfully with invalid credentials")]
    public async Task Should_NotLoginSuccessfully_WithInvalidCredentials()
    {
        // Arrange
        var credentials = new LoginCommand("admin", "Admin@123");

        // Act
        var response = await Client.PostAsJsonAsync("api/v1/auth/login", credentials);
        
        // Assert
        var loginResponse = await ReadResultValueAsync<LoginCommandResponse>(response);
        loginResponse.Should().NotBeNull();
        loginResponse.IsSuccess.Should().BeFalse();
        loginResponse.Value.Should().BeNull();
        loginResponse.Error.Should().NotBeNull();
        loginResponse.Error.Message.Should().Be(AuthenticationErrors.InvalidCredentials);
        loginResponse.Error.Type.Should().Be("BadRequest");
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should not login successfully with invalid password")]
    public async Task Should_NotLoginSuccessfully_WithInvalidPassword()
    {
        // Arrange
        var credentials = new LoginCommand("admin123", "WrongPassword@123");

        // Act
        var response = await Client.PostAsJsonAsync("api/v1/auth/login", credentials);
        
        // Assert
        var loginResponse = await ReadResultValueAsync<LoginCommandResponse>(response);
        loginResponse.Should().NotBeNull();
        loginResponse.IsSuccess.Should().BeFalse();
        loginResponse.Value.Should().BeNull();
        loginResponse.Error.Should().NotBeNull();
        loginResponse.Error.Message.Should().Be(AuthenticationErrors.InvalidCredentials);
        loginResponse.Error.Type.Should().Be("BadRequest");
    }

    #endregion

    #region GetPermissions Tests

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should get permissions for authenticated user")]
    public async Task Should_GetPermissions_ForAuthenticatedUser()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await Client.GetAsync("api/v1/auth/permissions");

        // Assert
        response.EnsureSuccessStatusCode();
        var permissionsResponse = await ReadResultValueAsync<GetPermissionsQueryResponse>(response);
        permissionsResponse.Should().NotBeNull();
        permissionsResponse.IsSuccess.Should().BeTrue();
        permissionsResponse.Value.Should().NotBeNull();
        permissionsResponse.Value.Permissions.Should().NotBeNull();
        permissionsResponse.Value.Permissions.Should().BeOfType<string[]>();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should not get permissions without authentication")]
    public async Task Should_NotGetPermissions_WithoutAuthentication()
    {
        // Arrange
        ClearAuthentication();

        // Act
        var response = await Client.GetAsync("api/v1/auth/permissions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should not get permissions with invalid token")]
    public async Task Should_NotGetPermissions_WithInvalidToken()
    {
        // Arrange
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid-token");

        // Act
        var response = await Client.GetAsync("api/v1/auth/permissions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region GetRefreshToken Tests

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should refresh token with valid refresh token")]
    public async Task Should_RefreshToken_WithValidRefreshToken()
    {
        // Arrange
        var loginResponse = await Client.PostAsJsonAsync("api/v1/auth/login", 
            new LoginCommand("admin123", "Admin@123"));
        
        var loginResult = await ReadResultValueAsync<LoginCommandResponse>(loginResponse);
        loginResult.Should().NotBeNull();
        loginResult.IsSuccess.Should().BeTrue();
        
        var refreshToken = loginResult.Value?.RefreshToken;
        refreshToken.Should().NotBeNullOrEmpty();
        
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", refreshToken);

        // Act
        var response = await Client.GetAsync("api/v1/auth/refresh");

        // Assert
        response.EnsureSuccessStatusCode();
        var refreshResponse = await ReadResultValueAsync<GetRefreshTokenQueryResponse>(response);
        refreshResponse.Should().NotBeNull();
        refreshResponse.IsSuccess.Should().BeTrue();
        refreshResponse.Value.Should().NotBeNull();
        refreshResponse.Value!.Token.Should().NotBeNullOrEmpty();
        refreshResponse.Value.RefreshToken.Should().NotBeNullOrEmpty();
        refreshResponse.Value.UserDomainId.Should().NotBeEmpty();
        refreshResponse.Value.UserApplicationId.Should().NotBeEmpty();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should not refresh token without refresh token")]
    public async Task Should_NotRefreshToken_WithoutRefreshToken()
    {
        // Arrange
        ClearAuthentication();

        // Act
        var response = await Client.GetAsync("api/v1/auth/refresh");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should not refresh token with invalid refresh token")]
    public async Task Should_NotRefreshToken_WithInvalidRefreshToken()
    {
        // Arrange
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid-refresh-token");

        // Act
        var response = await Client.GetAsync("api/v1/auth/refresh");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Logout Tests

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should logout successfully when authenticated")]
    public async Task Should_LogoutSuccessfully_WhenAuthenticated()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await Client.PostAsync("api/v1/auth/logout", null);

        // Assert
        response.EnsureSuccessStatusCode();
        var logoutResponse = await ReadResultValueAsync<object>(response);
        logoutResponse.Should().NotBeNull();
        logoutResponse.IsSuccess.Should().BeTrue();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should not logout without authentication")]
    public async Task Should_NotLogout_WithoutAuthentication()
    {
        // Arrange
        ClearAuthentication();

        // Act
        var response = await Client.PostAsync("api/v1/auth/logout", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion
}