using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Riber.Application.Abstractions.Services;
using Riber.Application.Exceptions;
using Riber.Domain.Constants;

namespace Riber.Infrastructure.Services.Authentication;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) 
    : ICurrentUserService
{
    private ClaimsPrincipal CurrentUser 
        => httpContextAccessor.HttpContext?.User!;
    
    public string[] GetPermissions()
        => [.. 
            CurrentUser.Claims
            .Where(x => x.Type == "permission")
            .Select(x => x.Value)
        ];

    public Guid GetUserId()
    {
        var userIdClaim = CurrentUser.FindFirst(ClaimTypes.NameIdentifier)!;
        return Guid.Parse(userIdClaim.Value);
    }

    public Guid GetCompanyId()
    {
        var companyIdClaim = CurrentUser.FindFirst("companyId")!;
        if (!Guid.TryParse(companyIdClaim.Value, out var companyId))
            throw new BadRequestException(ErrorMessage.Invalid.CompanyId);
        
        return companyId;
    }
}