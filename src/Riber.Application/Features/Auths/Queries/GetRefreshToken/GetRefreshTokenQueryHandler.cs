using System.Net;
using Riber.Application.Abstractions.Queries;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Common;
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
        if (!refreshResult)
            return Result.Failure<GetRefreshTokenQueryResponse>(AuthenticationErrors.InvalidCredentials);

        var user = await userQueryService.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure<GetRefreshTokenQueryResponse>(NotFoundErrors.User, HttpStatusCode.NotFound);

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