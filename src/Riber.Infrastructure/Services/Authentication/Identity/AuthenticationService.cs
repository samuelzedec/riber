using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Common;
using Riber.Application.Models.User;
using Riber.Domain.Constants.Messages.Common;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Services.Authentication.Identity;

public sealed class AuthenticationService(
    UserManager<ApplicationUser> userManager,
    UserMappingService userMappingService)
    : IAuthenticationService
{
    private IQueryable<ApplicationUser> GetBaseUserQuery
        => userManager.Users.Include(u => u.UserDomain);

    public async Task<Result<UserDetailsModel>> LoginAsync(string userNameOrEmail, string password)
    {
        var normalizedInput = userNameOrEmail.ToUpperInvariant();
        var user = await GetBaseUserQuery.FirstOrDefaultAsync(u =>
            u.NormalizedUserName == normalizedInput ||
            u.NormalizedEmail == normalizedInput);

        if (user is null)
            return Result.Failure<UserDetailsModel>(NotFoundErrors.User, HttpStatusCode.NotFound);

        if (!await userManager.CheckPasswordAsync(user, password))
            return Result.Failure<UserDetailsModel>(AuthenticationErrors.InvalidCredentials, HttpStatusCode.Unauthorized);

        await userManager.UpdateSecurityStampAsync(user);
        return Result.Success(await userMappingService.BuildUserDetailsAsync(user));
    }

    public async Task<Result<EmptyResult>> RefreshSecurityStampAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Failure<EmptyResult>(NotFoundErrors.User, HttpStatusCode.NotFound);
    
        await userManager.UpdateSecurityStampAsync(user);
        return Result.Success(new EmptyResult());
    } 
}