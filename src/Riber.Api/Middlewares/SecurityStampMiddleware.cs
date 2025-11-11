using System.Net;
using System.Security.Claims;
using Riber.Api.Extensions;
using Riber.Application.Abstractions.Services.Authentication;

namespace Riber.Api.Middlewares;

public sealed class SecurityStampMiddleware(
    IUserQueryService userQueryService)
    : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.User.Identity!.IsAuthenticated)
        {
            await next(context);
            return;
        }

        (string? userId, string? tokenSecurityStamp) = ExtractUserClaims(context.User);
        if (!Guid.TryParse(userId, out var userIdParse) || string.IsNullOrWhiteSpace(tokenSecurityStamp))
        {
            await context.WriteErrorResponse(HttpStatusCode.Unauthorized, "Token do usuário inválido ou mal formado.");
            return;
        }

        var user = await userQueryService.FindByIdAsync(userIdParse);
        if (user is null || user.SecurityStamp != tokenSecurityStamp)
        {
            await context.WriteErrorResponse(HttpStatusCode.Unauthorized, "Security stamp inválido ou usuário não encontrado.");
            return;
        }

        await next(context);
    }

    private static (string? userId, string? securityStamp) ExtractUserClaims(ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var securityStamp = user.FindFirst("securityStamp")?.Value;
        return (userId, securityStamp);
    }
}