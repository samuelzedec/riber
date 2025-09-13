using Riber.Application.Abstractions.Queries;

namespace Riber.Application.Features.Auths.Queries.GetRefreshToken;

public sealed record GetRefreshTokenQueryResponse(
    Guid UserApplicationId,
    Guid UserDomainId,
    string Token,
    string RefreshToken
) : IQueryResponse;