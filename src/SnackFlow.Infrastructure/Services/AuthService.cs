using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.DTOs;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Enums;
using SnackFlow.Infrastructure.Persistence.Identity;

namespace SnackFlow.Infrastructure.Services;

public sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ILogger<AuthService> logger)
    : IAuthService
{
    public async Task CreateAsync(CreateApplicationUserDTO userDto, CancellationToken cancellationToken)
    {
        try
        {
            var applicationUser = new ApplicationUser
            {
                Name = userDto.Name,
                UserName = userDto.Email,
                Email = userDto.Email,
                EmailConfirmed = false,
                PhoneNumber = userDto.PhoneNumber,
                UserDomainId = userDto.UserDomainId
            };
            var result = await userManager.CreateAsync(applicationUser, userDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Falha ao criar usuário no Identity: {errors}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessage.Exception.Unexpected(ex.GetType().Name, ex.Message));
            throw;
        }
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email)
                   ?? throw new InvalidOperationException(ErrorMessage.NotFound.User);

        return await userManager.CheckPasswordAsync(user, password);
    }

    public async Task<UserDetailsDTO?> FindByIdAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        return user is null ? null : await MapUserDetailsAsync(user);
    }

    public async Task<UserDetailsDTO?> FindByEmailAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user is null ? null : await MapUserDetailsAsync(user);
    }

    public async Task AssignRoleToUserAsync(string userId, BusinessPosition position)
    {
        var user = await userManager.FindByIdAsync(userId)
                   ?? throw new InvalidOperationException(ErrorMessage.NotFound.User);

        var roleName = position.ToString().ToUpperInvariant();
        await EnsureRoleExistsAsync(roleName);

        var result = await userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Falha ao adicionar role: {errors}");
        }
    }

    public async Task UpdateUserRoleAsync(string userId, BusinessPosition newPosition)
    {
        var user = await userManager.FindByIdAsync(userId)
                   ?? throw new InvalidOperationException(ErrorMessage.NotFound.User);

        var currentRoles = await userManager.GetRolesAsync(user);
        if (currentRoles.Any())
            await userManager.RemoveFromRolesAsync(user, currentRoles);

        await AssignRoleToUserAsync(userId, newPosition);
        await userManager.UpdateSecurityStampAsync(user);
    }

    public async Task EnsureRoleExistsAsync(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("Nome da função não pode ser vazio", nameof(roleName));

        if (!await roleManager.RoleExistsAsync(roleName))
            throw new InvalidOperationException("Está função não existe");
    }

    public async Task<IList<Claim>> GetClaimsByRoleName(string roleName)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        return role is null ? [] : await roleManager.GetClaimsAsync(role);
    }

    #region Helpers

    private async Task<UserDetailsDTO> MapUserDetailsAsync(ApplicationUser user)
    {
        var claims = new List<Claim>();
        claims.AddRange(await userManager.GetClaimsAsync(user));

        var roles = await userManager.GetRolesAsync(user);
        foreach (string role in roles)
            claims.AddRange(await GetClaimsByRoleName(role));

        return new UserDetailsDTO
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber!,
            SecurityStamp = user.SecurityStamp!,
            Roles = roles,
            Claims = [.. claims.Select(x => new ClaimDTO { Type = x.Type, Value = x.Value })]
        };
    }

    #endregion
}