using SnackFlow.Application.Abstractions.Queries;

namespace SnackFlow.Application.Features.Auths.Queries.GetRefreshToken;

public sealed record GetRefreshTokenQuery : IQuery<GetRefreshTokenQueryResponse>;