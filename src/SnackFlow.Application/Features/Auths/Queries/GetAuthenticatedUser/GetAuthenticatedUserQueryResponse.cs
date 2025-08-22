using SnackFlow.Application.Abstractions.Queries;

namespace SnackFlow.Application.Features.Auths.Queries.GetAuthenticatedUser;

public sealed record GetAuthenticatedUserQueryResponse(string[] Permissions) : IQueryResponse;