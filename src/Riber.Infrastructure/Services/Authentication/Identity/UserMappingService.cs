using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Riber.Application.Models.Auth;
using Riber.Application.Models.User;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Services.Authentication.Identity;

public sealed class UserMappingService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager)
{
    public async Task<UserDetailsModel> BuildUserDetailsAsync(ApplicationUser user)
    {
        var userClaims = await userManager.GetClaimsAsync(user);
        var userRoles = await userManager.GetRolesAsync(user);
        var roleClaims = await GetClaimsFromRolesAsync(userRoles);
        var allClaims = userClaims.Concat(roleClaims).ToList();

        return new UserDetailsModel(
            Id: user.Id,
            UserName: user.UserName!,
            Email: user.Email!,
            EmailConfirmed: user.EmailConfirmed,
            PhoneNumber: user.PhoneNumber!,
            SecurityStamp: user.SecurityStamp!,
            UserDomainId: user.UserDomainId,
            Roles: [..userRoles],
            Claims: [..allClaims.Select(claim => new ClaimModel(claim.Type, claim.Value))],
            UserDomain: user.UserDomain
        );
    }

    private async Task<IEnumerable<Claim>> GetClaimsFromRolesAsync(IList<string> roleNames)
    {
        var allRoleClaims = new List<Claim>();
        foreach (var roleName in roleNames)
        {
            var roleClaims = await GetClaimsByRoleNameAsync(roleName);
            allRoleClaims.AddRange(roleClaims);
        }

        return allRoleClaims;
    }

    private async Task<IList<Claim>> GetClaimsByRoleNameAsync(string roleName)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        return role is not null ? await roleManager.GetClaimsAsync(role) : [];
    }
}