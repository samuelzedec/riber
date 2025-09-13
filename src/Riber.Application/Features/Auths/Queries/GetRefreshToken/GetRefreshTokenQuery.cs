using Riber.Application.Abstractions.Queries;

namespace Riber.Application.Features.Auths.Queries.GetRefreshToken;

public sealed record GetRefreshTokenQuery : IQuery<GetRefreshTokenQueryResponse>;