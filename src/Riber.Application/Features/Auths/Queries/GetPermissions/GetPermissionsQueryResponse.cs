using Riber.Application.Abstractions.Queries;

namespace Riber.Application.Features.Auths.Queries.GetPermissions;

public sealed record GetPermissionsQueryResponse(string[] Permissions) : IQueryResponse;