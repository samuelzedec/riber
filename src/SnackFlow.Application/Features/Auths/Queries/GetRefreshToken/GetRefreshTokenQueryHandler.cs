using Microsoft.Extensions.Logging;
using SnackFlow.Application.Abstractions.Queries;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.Common;
using SnackFlow.Application.Exceptions;
using SnackFlow.Domain.Constants;

namespace SnackFlow.Application.Features.Auths.Queries.GetRefreshToken;

internal sealed class GetRefreshTokenQueryHandler(
    ICurrentUserService currentUserService,
    IAuthService authService,
    ITokenService tokenService,
    ILogger<GetRefreshTokenQueryHandler> logger)
    : IQueryHandler<GetRefreshTokenQuery, GetRefreshTokenQueryResponse>
{
    public async ValueTask<Result<GetRefreshTokenQueryResponse>> Handle(GetRefreshTokenQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var userId = currentUserService.GetUserId()?.ToString()
                ?? throw new UnauthorizedException(ErrorMessage.Invalid.Auth);

            var user = await authService.UpdateSecurityStampAndGetUserAsync(userId);
            var token = tokenService.GenerateToken(user);
            var refreshToken = tokenService.GenerateRefreshToken(user.Id, user.SecurityStamp);

            return new GetRefreshTokenQueryResponse(
                UserApplicationId: user.Id,
                UserDomainId: user.UserDomainId,
                Token: token,
                RefreshToken: refreshToken
            );
        }
        catch (Exception ex) when (ex is not UnauthorizedException)
        {
            logger.LogError(ex, ErrorMessage.Exception.Unexpected(ex.GetType().Name, ex.Message));
            throw;
        }
    }
}