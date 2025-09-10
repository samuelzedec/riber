using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SnackFlow.Application.Abstractions.Services;

namespace SnackFlow.Infrastructure.Services.Authentication;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) 
    : ICurrentUserService
{
    private ClaimsPrincipal? CurrentUser 
        => httpContextAccessor.HttpContext?.User;
    
    public string[] GetPermissions()
        => CurrentUser is null ? [] : [.. 
            CurrentUser.Claims
            .Where(x => x.Type == "permission")
            .Select(x => x.Value)
        ];

    public Guid? GetUserId()
    {
        if (CurrentUser is null)
            return null;

        var userIdClaim = CurrentUser.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim is null)
            return null;

        if (Guid.TryParse(userIdClaim.Value, out var userId) && userId != Guid.Empty)
            return userId;

        return null;
    }
}