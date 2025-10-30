using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Dtos.User;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Services.Authentication.Identity;

public sealed class AuthenticationService(
    UserManager<ApplicationUser> userManager,
    UserMappingService userMappingService)
    : IAuthenticationService
{
    private IQueryable<ApplicationUser> GetBaseUserQuery
        => userManager.Users.Include(u => u.UserDomain);

    public async Task<UserDetailsDto?> LoginAsync(string userNameOrEmail, string password)
    {
        var normalizedInput = userNameOrEmail.ToUpperInvariant();
        var user = await GetBaseUserQuery.FirstOrDefaultAsync(u =>
            u.NormalizedUserName == normalizedInput ||
            u.NormalizedEmail == normalizedInput);

        if (user is null)
            return null;

        if (!await userManager.CheckPasswordAsync(user, password))
            return null;
        
        await userManager.UpdateSecurityStampAsync(user);
        return await userMappingService.BuildUserDetailsAsync(user);
    }

    public async Task<bool> RefreshSecurityStampAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user is not null && (await userManager.UpdateSecurityStampAsync(user)).Succeeded;
    } 
}