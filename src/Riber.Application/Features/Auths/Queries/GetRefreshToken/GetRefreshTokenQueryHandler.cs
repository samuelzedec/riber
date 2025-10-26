using Riber.Application.Abstractions.Queries;
using Riber.Application.Abstractions.Services;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Common;
using Riber.Application.Exceptions;
using Riber.Domain.Constants.Messages.Common;

namespace Riber.Application.Features.Auths.Queries.GetRefreshToken;

internal sealed class GetRefreshTokenQueryHandler(
    ICurrentUserService currentUserService,
    IAuthenticationService authenticationService,
    IUserQueryService userQueryService,
    ITokenService tokenService)
    : IQueryHandler<GetRefreshTokenQuery, GetRefreshTokenQueryResponse>
{
    public async ValueTask<Result<GetRefreshTokenQueryResponse>> Handle(GetRefreshTokenQuery query,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var refreshResult = await authenticationService.RefreshSecurityStampAsync(userId.ToString());
        if (!refreshResult.IsSuccess)
            return Result.Failure<GetRefreshTokenQueryResponse>(refreshResult.Error.Message, refreshResult.StatusCode);

        var userResult = await userQueryService.FindByIdAsync(userId);
        if (!userResult.IsSuccess)
            return Result.Failure<GetRefreshTokenQueryResponse>(userResult.Error.Message, userResult.StatusCode);

        var user = userResult.Value!;
        var token = tokenService.GenerateToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user.Id, user.SecurityStamp);

        return new GetRefreshTokenQueryResponse(
            UserApplicationId: user.Id,
            UserDomainId: user.UserDomainId,
            Token: token,
            RefreshToken: refreshToken
        );
    }
}