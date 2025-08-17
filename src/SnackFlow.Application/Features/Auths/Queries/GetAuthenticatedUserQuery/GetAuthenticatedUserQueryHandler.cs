using SnackFlow.Application.Abstractions.Queries;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.Common;

namespace SnackFlow.Application.Features.Auths.Queries.GetAuthenticatedUserQuery;

internal sealed class GetAuthenticatedUserQueryHandler(
    ICurrentUserService currentUserService) 
    : IQueryHandler<GetAuthenticatedUserQuery, GetAuthenticatedUserQueryResponse>
{
    public ValueTask<Result<GetAuthenticatedUserQueryResponse>> Handle(GetAuthenticatedUserQuery query, CancellationToken cancellationToken)
    {
        var permissions = currentUserService.GetPermissions();
        
        return new ValueTask<Result<GetAuthenticatedUserQueryResponse>>(
            Result.Success(new GetAuthenticatedUserQueryResponse(permissions)));
    }
}