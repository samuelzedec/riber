using SnackFlow.Api.Common.Extensions;
using SnackFlow.Domain.Repositories;

namespace SnackFlow.Api.Middlewares;

internal sealed class CompanyRequiredMiddleware(IUserRepository userRepository) : IMiddleware
{
    private readonly string[] _allowedPaths = ["/api/companies", "/api/auth/logout", "/api/auth/me"];

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.User.Identity!.IsAuthenticated)
        {
            await next(context);
            return;
        }
        
        var userDomainId = context.User.FindFirst("user_domain_id")?.Value;
        if (string.IsNullOrEmpty(userDomainId))
        {
            await next(context);
            return;
        }

        var domainUser = await userRepository
            .GetSingleAsync(x => x.Id == Guid.Parse(userDomainId));
        
        if (domainUser is { CompanyId: null } || !_allowedPaths.Any(path => context.Request.Path.StartsWithSegments(path)))
        {
            await context.WriteUnauthorizedResponse(
                title: "COMPANY_REQUIRED",
                message: "Você precisa estar associado a uma empresa!",
                code: 403
            );
        }
        
        await next(context);
    }
}