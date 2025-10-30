using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Dtos.User;
using Riber.Infrastructure.Extensions;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Services.Authentication.Identity;

public sealed class UserManagementService(
    UserManager<ApplicationUser> userManager,
    IRoleManagementService roleManagementService,
    ILogger<UserManagementService> logger)
    : IUserManagementService
{
    public async Task<bool> CreateUserAsync(CreateApplicationUserDto dto)
    {
        if (dto.Roles.Count == 0)
            return false;

        var applicationUser = new ApplicationUser
        {
            Name = dto.Name,
            UserName = dto.UserName,
            Email = dto.Email,
            EmailConfirmed = false,
            PhoneNumber = dto.PhoneNumber,
            UserDomainId = dto.UserDomainId
        };

        var createResult = await userManager.CreateAsync(applicationUser, dto.Password);
        if (!createResult.Succeeded)
        {
            createResult.LogIdentityErrors($"Falha ao criar usuário {dto.UserName}", logger);
            return false;
        }

        var roleResult = await roleManagementService.AssignRoleAsync(
            applicationUser.Id.ToString(), dto.Roles.First());

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