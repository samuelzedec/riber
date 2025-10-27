using Riber.Application.Abstractions.Queries;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Common;

namespace Riber.Application.Features.Auths.Queries.GetPermissions;

internal sealed class GetPermissionsQueryHandler(
    ICurrentUserService currentUserService) 
    : IQueryHandler<GetPermissionsQuery, GetPermissionsQueryResponse>
{
    public ValueTask<Result<GetPermissionsQueryResponse>> Handle(GetPermissionsQuery query, CancellationToken cancellationToken)
    {
        var permissions = currentUserService.GetPermissions();
        
        return new ValueTask<Result<GetPermissionsQueryResponse>>(
            Result.Success(new GetPermissionsQueryResponse(permissions)));
    }
}