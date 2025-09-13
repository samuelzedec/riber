using Riber.Application.Abstractions.Queries;

namespace Riber.Application.Features.Auths.Queries.GetAuthenticatedUser;

public sealed record GetAuthenticatedUserQuery : IQuery<GetAuthenticatedUserQueryResponse>;