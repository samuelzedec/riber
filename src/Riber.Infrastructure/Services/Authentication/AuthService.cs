using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Riber.Application.Abstractions.Services;
using Riber.Application.Exceptions;
using Riber.Application.Models;
using Riber.Domain.Constants.Messages.Common;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Services.Authentication;

public sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager)
    : IAuthService
{
    #region Properties

    private IQueryable<ApplicationUser> GetBaseUserQuery
        => userManager.Users.AsNoTracking().Include(u => u.UserDomain);

    #endregion

    #region Methods

    public async Task CreateAsync(CreateApplicationUserModel applicationUserModel, CancellationToken cancellationToken)
    {
        var applicationUser = new ApplicationUser
        {
            Name = applicationUserModel.Name,
            UserName = applicationUserModel.UserName,
            Email = applicationUserModel.Email,
            EmailConfirmed = false,
            PhoneNumber = applicationUserModel.PhoneNumber,
            UserDomainId = applicationUserModel.UserDomainId
        };
        var result = await userManager.CreateAsync(applicationUser, applicationUserModel.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Falha ao criar usuário no Identity: {errors}");
        }

        await userManager.AddToRoleAsync(applicationUser, applicationUserModel.Roles.ToList()[0]);
    }

    public async Task<UserDetailsModel?> LoginAsync(string userNameOrEmail, string password)
    {
        var normalizedInput = userNameOrEmail.ToUpperInvariant();
        var user = await GetBaseUserQuery
            .FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedInput || u.NormalizedEmail == normalizedInput)
            ?? throw new NotFoundException(NotFoundErrors.User);

        return await userManager.CheckPasswordAsync(user, password)
            ? await MapUserDetailsAsync(user)
            : null;
    }

    public async Task<UserDetailsModel?> FindByIdAsync(string userId)
    {
        var inputParsed = Guid.Parse(userId);
        var user = await GetBaseUserQuery
            .FirstOrDefaultAsync(u => u.Id == inputParsed);

        return user is not null
            ? await MapUserDetailsAsync(user)
            : null;
    }

    public async Task<UserDetailsModel?> FindByEmailAsync(string email)
    {
        var normalizedEmail = email.ToUpperInvariant();
        var user = await GetBaseUserQuery
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

        return user is not null
            ? await MapUserDetailsAsync(user)
            : null;
    }

    public async Task<UserDetailsModel?> FindByUserNameAsync(string userName)
    {
        var normalizedUserName = userName.ToUpperInvariant();
        var user = await GetBaseUserQuery
            .FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName);

        return user is not null
            ? await MapUserDetailsAsync(user)
            : null;
    }

    public async Task<UserDetailsModel?> FindByPhoneAsync(string phoneNumber)
    {
        var user = await GetBaseUserQuery
            .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);

        return user is not null
            ? await MapUserDetailsAsync(user)
            : null;
    }

    public async Task AssignRoleToUserAsync(string userId, string roleName)
    {
        var user = await userManager.FindByIdAsync(userId)
                   ?? throw new InvalidOperationException(NotFoundErrors.User);

        await EnsureRoleExistsAsync(roleName);
        var result = await userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Falha ao adicionar role: {errors}");
        }
    }

    public async Task UpdateUserRoleAsync(string userId, string newRole)
    {
        var user = await userManager.FindByIdAsync(userId)
                   ?? throw new InvalidOperationException(NotFoundErrors.User);

        var currentRoles = await userManager.GetRolesAsync(user);
        if (currentRoles.Any())
            await userManager.RemoveFromRolesAsync(user, currentRoles);

        await AssignRoleToUserAsync(userId, newRole);
        await userManager.UpdateSecurityStampAsync(user);
    }

    public async Task EnsureRoleExistsAsync(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            throw new BadRequestException("Nome da função não pode ser vazio.");

        if (!await roleManager.RoleExistsAsync(roleName))
            throw new NotFoundException("Está função não existe.");
    }
    
    public async Task RefreshUserSecurityAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is not null)
            await userManager.UpdateSecurityStampAsync(user);
    }

    #endregion

    #region Helpers

    private async Task<UserDetailsModel> MapUserDetailsAsync(ApplicationUser user)
    {
        var claims = new List<Claim>();
        claims.AddRange(await userManager.GetClaimsAsync(user));

        var roles = await userManager.GetRolesAsync(user);
        foreach (string role in roles)
            claims.AddRange(await GetClaimsByRoleName(role));

        return new UserDetailsModel(
            Id: user.Id,
            UserName: user.UserName!,
            Email: user.Email!,
            EmailConfirmed: user.EmailConfirmed,
            PhoneNumber: user.PhoneNumber!,
            SecurityStamp: user.SecurityStamp!,
            UserDomainId: user.UserDomainId,
            Roles: [.. roles],
            Claims: [.. claims.Select(x => new ClaimModel(Type: x.Type, Value: x.Value))],
            UserDomain: user.UserDomain
        );
    }

    private async Task<IList<Claim>> GetClaimsByRoleName(string roleName)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        return role is null ? [] : await roleManager.GetClaimsAsync(role);
    }

    #endregion
}