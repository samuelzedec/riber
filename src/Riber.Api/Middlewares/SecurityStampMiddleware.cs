using System.Security.Claims;
using Riber.Api.Extensions;
using Riber.Application.Abstractions.Services;

namespace Riber.Api.Middlewares;

public sealed class SecurityStampMiddleware(
    IAuthService authService,
    ILogger<SecurityStampMiddleware> logger)
    : IMiddleware
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
        
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(tokenSecurityStamp))
        {
            logger.LogError("Valores: userId = {UserId}, securityStamp = {TokenSecurityStamp}", userId, tokenSecurityStamp);
            await context.WriteUnauthorizedResponse(
                StatusCodes.Status401Unauthorized,
                "Token do usuário inválido ou mal formado."
            );
            return;
        }
        
        logger.LogWarning("Validando os dados do usuário com ID {UserId} e security stamp {TokenSecurityStamp}", userId, tokenSecurityStamp);
        var user = await authService.FindByIdAsync(userId);
        if (user is null || user.SecurityStamp != tokenSecurityStamp)
        {
            await context.WriteUnauthorizedResponse(
                code: StatusCodes.Status401Unauthorized,
                messages: "Security stamp inválido ou usuário não encontrado."
            );
            return;
        }
        await next(context);
    }
}