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
                "Acesso não autorizado",
                "Token do usuário inválido ou mal formado.",
                StatusCodes.Status401Unauthorized
            );
            return;
        }
        
        logger.LogWarning("Validando os dados do usuário com ID {UserId} e security stamp {TokenSecurityStamp}", userId, tokenSecurityStamp);
        var user = await authService.FindByIdAsync(userId);
        if (user is null || user.SecurityStamp != tokenSecurityStamp)
        {
            await context.WriteUnauthorizedResponse(
                title: "Acesso não autorizado",
                message: "Security stamp inválido ou usuário não encontrado.",
                code: StatusCodes.Status401Unauthorized
            );
            return;
        }
        await next(context);
    }
}