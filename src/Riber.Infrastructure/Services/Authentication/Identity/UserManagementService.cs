using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Common;
using Riber.Application.Models.User;
using Riber.Domain.Constants.Messages.Common;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Services.Authentication.Identity;

public sealed class UserManagementService(
    UserManager<ApplicationUser> userManager,
    IRoleManagementService roleManagementService,
    ILogger<UserManagementService> logger)
    : IUserManagementService
{
    public async Task<Result<EmptyResult>> CreateUserAsync(CreateApplicationUserModel model)
    {
        if (model.Roles.Count == 0)
            return Result.Failure<EmptyResult>("Pelo menos uma role deve ser especificada");

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
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            logger.LogError("Falha ao criar usuário {UserName}: {Errors}", model.UserName, errors);
            return Result.Failure<EmptyResult>("Falha ao criar usuário");
        }

        var roleResult = await roleManagementService.AssignRoleAsync(
            applicationUser.Id.ToString(),
            model.Roles.First());

        if (roleResult.IsSuccess)
            return Result.Success(new EmptyResult());

        await userManager.DeleteAsync(applicationUser);
        return Result.Failure<EmptyResult>("Falha ao configurar permissões do usuário");
    }

    public async Task<Result<EmptyResult>> DeleteUserAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Failure<EmptyResult>(NotFoundErrors.User, HttpStatusCode.NotFound);

        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded)
            return Result.Success(new EmptyResult());

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        logger.LogError("Falha ao deletar usuário {UserId}: {Errors}", userId, errors);
        return Result.Failure<EmptyResult>("Falha ao deletar usuário", HttpStatusCode.InternalServerError);
    }
}