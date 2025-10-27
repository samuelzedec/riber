using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Infrastructure.Extensions;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Services.Authentication.Identity;

public sealed class RoleManagementService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ILogger<RoleManagementService> logger)
    : IRoleManagementService
{
    public async Task<bool> AssignRoleAsync(string userId, string roleName)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return false;

        if (!await roleManager.RoleExistsAsync(roleName))
            return false;

        var result = await userManager.AddToRoleAsync(user, roleName);
        if (result.Succeeded)
            return true;

        result.LogIdentityErrors($"Falha ao adicionar role {roleName} para usuário {userId}", logger);
        return false;
    }

    public async Task<bool> UpdateUserRoleAsync(string userId, string newRole)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return false;

        var currentRoles = await userManager.GetRolesAsync(user);
        if (currentRoles.Any())
        {
            var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                removeResult.LogIdentityErrors($"Falha ao remover roles do usuário {userId}", logger);
                return false;
            }
        }

        var assignResult = await AssignRoleAsync(userId, newRole);
        if (!assignResult)
            return assignResult;

        await userManager.UpdateSecurityStampAsync(user);
        return true;
    }
}