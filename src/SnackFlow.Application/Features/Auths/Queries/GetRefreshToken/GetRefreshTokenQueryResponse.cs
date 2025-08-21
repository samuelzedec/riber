using SnackFlow.Application.Abstractions.Queries;

namespace SnackFlow.Application.Features.Auths.Queries.GetRefreshToken;

public sealed record GetRefreshTokenQueryResponse(
    Guid UserApplicationId,
    Guid UserDomainId,
    string Token,
    string RefreshToken
) : IQueryResponse;