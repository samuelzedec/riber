using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Models.User;
using Riber.Infrastructure.Extensions;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Services.Authentication.Identity;

public sealed class UserManagementService(
    UserManager<ApplicationUser> userManager,
    IRoleManagementService roleManagementService,
    ILogger<UserManagementService> logger)
    : IUserManagementService
{
    public async Task<bool> CreateUserAsync(CreateApplicationUserModel model)
    {
        if (model.Roles.Count == 0)
            return false;

        var applicationUser = new ApplicationUser
        {
            Name = model.Name,
            UserName = model.UserName,
            Email = model.Email,
            EmailConfirmed = false,
            PhoneNumber = model.PhoneNumber,
            UserDomainId = model.UserDomainId
        };

        var createResult = await userManager.CreateAsync(applicationUser, model.Password);
        if (!createResult.Succeeded)
        {
            createResult.LogIdentityErrors($"Falha ao criar usuário {model.UserName}", logger);
            return false;
        }

        var roleResult = await roleManagementService.AssignRoleAsync(
            applicationUser.Id.ToString(), model.Roles.First());

        if (roleResult)
            return true;

        await userManager.DeleteAsync(applicationUser);
        return false;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return false;

        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded)
            return true;

        result.LogIdentityErrors($"Falha ao deletar usuário {userId}", logger);
        return false;
    }
}