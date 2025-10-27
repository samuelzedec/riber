using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Models.User;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Services.Authentication.Identity;

public sealed class UserQueryService(
    UserManager<ApplicationUser> userManager,
    UserMappingService userMappingService)
    : IUserQueryService
{
    private IQueryable<ApplicationUser> GetBaseUserQuery
        => userManager.Users.AsNoTracking().Include(u => u.UserDomain);

    public async Task<UserDetailsModel?> FindByIdAsync(Guid userId)
    {
        var user = await GetBaseUserQuery.FirstOrDefaultAsync(u => u.Id == userId);
        return await BuildUserDetailsAsync(user);
    }

    public async Task<UserDetailsModel?> FindByEmailAsync(string email)
    {
        var normalizedEmail = email.ToUpperInvariant();
        var user = await GetBaseUserQuery.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
        return await BuildUserDetailsAsync(user);
    }

    public async Task<UserDetailsModel?> FindByUserNameAsync(string userName)
    {
        var normalizedUserName = userName.ToUpperInvariant();
        var user = await GetBaseUserQuery.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName);
        return await BuildUserDetailsAsync(user);
    }

    public async Task<UserDetailsModel?> FindByPhoneAsync(string phoneNumber)
    {
        var user = await GetBaseUserQuery.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        return await BuildUserDetailsAsync(user);
    }

    private async Task<UserDetailsModel?> BuildUserDetailsAsync(ApplicationUser? user)
        => user is null ? null : await userMappingService.BuildUserDetailsAsync(user);
}