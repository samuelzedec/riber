using System.Net.Http.Json;
using FluentAssertions;
using Riber.Api.Tests.Fixtures;
using Riber.Application.Features.Auths.Commands.Login;
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
        response.EnsureSuccessStatusCode();
        var loginResponse = await ReadResultValueAsync<LoginCommandResponse>(response);
        loginResponse.Should().NotBeNull();
        loginResponse.Value.Should().NotBeNull();
        loginResponse.IsSuccess.Should().BeTrue();
        loginResponse.Value.Token.Should().NotBeNullOrEmpty();
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
        loginResponse.Error.Type.Should().Be("NOT_FOUND");
        loginResponse.Error.Message.Should().NotBeNullOrEmpty();
        loginResponse.Error.Details.Should().BeNullOrEmpty();
    }
    
    #endregion
}