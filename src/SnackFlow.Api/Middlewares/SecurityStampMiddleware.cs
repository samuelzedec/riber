using System.Security.Claims;
using SnackFlow.Api.Common.Extensions;
using SnackFlow.Application.Abstractions.Services;

namespace SnackFlow.Api.Middlewares;

public class SecurityStampMiddleware(IAuthService authService) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.User.Identity!.IsAuthenticated)
        {
            await next(context);
            return;
        }
        
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var tokenSecurityStamp = context.User.FindFirst("securityStamp")?.Value;
        
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tokenSecurityStamp))
        {
            await next(context);
            return;
        }

        var user = await authService.FindByIdAsync(Guid.Parse(userId));
        if (user is null || user.SecurityStamp != tokenSecurityStamp)
        {
            await context.WriteUnauthorizedResponse(
                title: "INVALID_TOKEN",
                message: "Token inválido ou expirado",
                code: 401
            );
            return;
        }

        await next(context);
    }
}