using SnackFlow.Application.Abstractions.Queries;

namespace SnackFlow.Application.Features.Auths.Queries.GetAuthenticatedUserQuery;

public sealed record GetAuthenticatedUserQuery : IQuery<GetAuthenticatedUserQueryResponse>;