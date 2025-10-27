using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Riber.Api.Tests;
using Riber.Api.Tests.Fixtures;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Tests.Services.Authentication.Identity;

public sealed class RoleManagementServiceTests : IntegrationTestBase
{
    private readonly IRoleManagementService _roleManagementService;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleManagementServiceTests(WebAppFixture webAppFixture, DatabaseFixture databaseFixture) 
        : base(webAppFixture, databaseFixture)
    {
        var scope = webAppFixture.GetFactory().Services.CreateScope();
        _roleManagementService = scope.ServiceProvider.GetRequiredService<IRoleManagementService>();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    }

    #region AssignRoleAsync Tests

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should assign role successfully to user without roles")]
    public async Task AssignRoleAsync_WithValidUserAndRole_ShouldReturnTrue()
    {
        // Arrange
        const string userId = "e6fba186-c1e7-4083-90a6-966c421720e5";
        const string roleName = "Employee";

        var user = await _userManager.FindByIdAsync(userId);
        var currentRoles = await _userManager.GetRolesAsync(user!);
        if (currentRoles.Any())
            await _userManager.RemoveFromRolesAsync(user!, currentRoles);

        // Act
        var result = await _roleManagementService.AssignRoleAsync(userId, roleName);

        // Assert
        result.Should().BeTrue();
        var roles = await _userManager.GetRolesAsync(user!);
        roles.Should().Contain(roleName);
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should assign additional role to user with existing roles")]
    public async Task AssignRoleAsync_WithUserHavingExistingRoles_ShouldAddNewRole()
    {
        // Arrange
        const string userId = "e6fba186-c1e7-4083-90a6-966c421720e5";
        const string existingRole = "Manager";
        const string newRole = "Viewer";

        var user = await _userManager.FindByIdAsync(userId);
        var currentRoles = await _userManager.GetRolesAsync(user!);
        if (currentRoles.Any())
            await _userManager.RemoveFromRolesAsync(user!, currentRoles);
        
        await _userManager.AddToRoleAsync(user!, existingRole);

        // Act
        var result = await _roleManagementService.AssignRoleAsync(userId, newRole);

        // Assert
        result.Should().BeTrue();
        var roles = await _userManager.GetRolesAsync(user!);
        roles.Should().Contain(existingRole);
        roles.Should().Contain(newRole);
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should return false when user does not exist")]
    public async Task AssignRoleAsync_WithNonExistentUser_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        const string roleName = "Admin";

        // Act
        var result = await _roleManagementService.AssignRoleAsync(userId, roleName);

        // Assert
        result.Should().BeFalse();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should return false when role does not exist")]
    public async Task AssignRoleAsync_WithNonExistentRole_ShouldReturnFalse()
    {
        // Arrange
        const string userId = "e6fba186-c1e7-4083-90a6-966c421720e5";
        const string roleName = "NonExistentRole";

        // Act
        var result = await _roleManagementService.AssignRoleAsync(userId, roleName);

        // Assert
        result.Should().BeFalse();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should handle assigning role that user already has")]
    public async Task AssignRoleAsync_WithRoleAlreadyAssigned_ShouldReturnFalse()
    {
        // Arrange
        const string userId = "e6fba186-c1e7-4083-90a6-966c421720e5";
        const string roleName = "Admin";

        var user = await _userManager.FindByIdAsync(userId);
        var currentRoles = await _userManager.GetRolesAsync(user!);
        if (currentRoles.Any())
            await _userManager.RemoveFromRolesAsync(user!, currentRoles);
        
        await _userManager.AddToRoleAsync(user!, roleName);

        // Act
        var result = await _roleManagementService.AssignRoleAsync(userId, roleName);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region UpdateUserRoleAsync Tests

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should update user role successfully when user has existing role")]
    public async Task UpdateUserRoleAsync_WithExistingRole_ShouldReplaceWithNewRole()
    {
        // Arrange
        const string userId = "e6fba186-c1e7-4083-90a6-966c421720e5";
        const string oldRole = "Employee";
        const string newRole = "Manager";

        var user = await _userManager.FindByIdAsync(userId);
        var currentRoles = await _userManager.GetRolesAsync(user!);
        if (currentRoles.Any())
            await _userManager.RemoveFromRolesAsync(user!, currentRoles);
        
        await _userManager.AddToRoleAsync(user!, oldRole);
        var securityStampBefore = user!.SecurityStamp;

        // Act
        var result = await _roleManagementService.UpdateUserRoleAsync(userId, newRole);

        // Assert
        result.Should().BeTrue();
        
        var updatedUser = await _userManager.FindByIdAsync(userId);
        var roles = await _userManager.GetRolesAsync(updatedUser!);
        
        roles.Should().NotContain(oldRole);
        roles.Should().Contain(newRole);
        roles.Should().HaveCount(1);
        updatedUser!.SecurityStamp.Should().NotBe(securityStampBefore);
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should update user role when user has multiple roles")]
    public async Task UpdateUserRoleAsync_WithMultipleExistingRoles_ShouldReplaceAllWithNewRole()
    {
        // Arrange
        const string userId = "e6fba186-c1e7-4083-90a6-966c421720e5";
        const string newRole = "Admin";

        var user = await _userManager.FindByIdAsync(userId);
        var currentRoles = await _userManager.GetRolesAsync(user!);
        if (currentRoles.Any())
            await _userManager.RemoveFromRolesAsync(user!, currentRoles);
        
        await _userManager.AddToRoleAsync(user!, "Manager");
        await _userManager.AddToRoleAsync(user!, "Employee");

        // Act
        var result = await _roleManagementService.UpdateUserRoleAsync(userId, newRole);

        // Assert
        result.Should().BeTrue();
        
        var updatedUser = await _userManager.FindByIdAsync(userId);
        var roles = await _userManager.GetRolesAsync(updatedUser!);
        
        roles.Should().HaveCount(1);
        roles.Should().Contain(newRole);
        roles.Should().NotContain("Manager");
        roles.Should().NotContain("Employee");
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should assign role to user without existing roles")]
    public async Task UpdateUserRoleAsync_WithNoExistingRoles_ShouldAssignNewRole()
    {
        // Arrange
        const string userId = "e6fba186-c1e7-4083-90a6-966c421720e5";
        const string newRole = "Viewer";

        var user = await _userManager.FindByIdAsync(userId);
        var currentRoles = await _userManager.GetRolesAsync(user!);
        if (currentRoles.Any())
            await _userManager.RemoveFromRolesAsync(user!, currentRoles);

        var securityStampBefore = user!.SecurityStamp;

        // Act
        var result = await _roleManagementService.UpdateUserRoleAsync(userId, newRole);

        // Assert
        result.Should().BeTrue();
        
        var updatedUser = await _userManager.FindByIdAsync(userId);
        var roles = await _userManager.GetRolesAsync(updatedUser!);
        
        roles.Should().HaveCount(1);
        roles.Should().Contain(newRole);
        updatedUser!.SecurityStamp.Should().NotBe(securityStampBefore);
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should return false when user does not exist")]
    public async Task UpdateUserRoleAsync_WithNonExistentUser_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        const string newRole = "Admin";

        // Act
        var result = await _roleManagementService.UpdateUserRoleAsync(userId, newRole);

        // Assert
        result.Should().BeFalse();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should return false when new role does not exist")]
    public async Task UpdateUserRoleAsync_WithNonExistentRole_ShouldReturnFalse()
    {
        // Arrange
        const string userId = "e6fba186-c1e7-4083-90a6-966c421720e5";
        const string newRole = "InvalidRole";

        // Act
        var result = await _roleManagementService.UpdateUserRoleAsync(userId, newRole);

        // Assert
        result.Should().BeFalse();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should update security stamp after role update")]
    public async Task UpdateUserRoleAsync_OnSuccess_ShouldUpdateSecurityStamp()
    {
        // Arrange
        const string userId = "e6fba186-c1e7-4083-90a6-966c421720e5";
        const string newRole = "Employee";

        var user = await _userManager.FindByIdAsync(userId);
        var securityStampBefore = user!.SecurityStamp;

        // Small delay to ensure timestamp difference
        await Task.Delay(100);

        // Act
        var result = await _roleManagementService.UpdateUserRoleAsync(userId, newRole);

        // Assert
        result.Should().BeTrue();
        
        var updatedUser = await _userManager.FindByIdAsync(userId);
        updatedUser!.SecurityStamp.Should().NotBe(securityStampBefore);
    }

    #endregion
}