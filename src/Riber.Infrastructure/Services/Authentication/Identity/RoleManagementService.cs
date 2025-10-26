using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Common;
using Riber.Domain.Constants.Messages.Common;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Services.Authentication.Identity;

public sealed class RoleManagementService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ILogger<RoleManagementService> logger)
    : IRoleManagementService
{
    public async Task<Result<EmptyResult>> AssignRoleAsync(string userId, string roleName)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Failure<EmptyResult>(NotFoundErrors.User, HttpStatusCode.NotFound);

        var roleExistsResult = await EnsureRoleExistsAsync(roleName);
        if (!roleExistsResult.IsSuccess)
            return roleExistsResult;

        var result = await userManager.AddToRoleAsync(user, roleName);
        if (result.Succeeded)
            return Result.Success(new EmptyResult());

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        logger.LogError("Falha ao adicionar role {RoleName} para usuário {UserId}: {Errors}", roleName, userId, errors);
        return Result.Failure<EmptyResult>("Falha ao adicionar role");
    }

    public async Task<Result<EmptyResult>> UpdateUserRoleAsync(string userId, string newRole)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Failure<EmptyResult>(NotFoundErrors.User, HttpStatusCode.NotFound);

        var roleExistsResult = await EnsureRoleExistsAsync(newRole);
        if (!roleExistsResult.IsSuccess)
            return roleExistsResult;

        var currentRoles = await userManager.GetRolesAsync(user);
        if (currentRoles.Any())
        {
            var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                logger.LogError("Falha ao remover roles do usuário {UserId}: {Errors}", userId, errors);
                return Result.Failure<EmptyResult>("Falha ao remover roles atuais");
            }
        }

        var assignResult = await AssignRoleAsync(userId, newRole);
        if (!assignResult.IsSuccess)
            return assignResult;

        await userManager.UpdateSecurityStampAsync(user);
        return Result.Success(new EmptyResult());
    }

    public async Task<Result<EmptyResult>> EnsureRoleExistsAsync(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return Result.Failure<EmptyResult>("Nome da role não pode ser vazio");

        if (!await roleManager.RoleExistsAsync(roleName))
            return Result.Failure<EmptyResult>("Esta role não existe", HttpStatusCode.NotFound);

        return Result.Success(new EmptyResult());
    }
}