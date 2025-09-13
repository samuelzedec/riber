using Riber.Application.Abstractions.Queries;
using Riber.Application.Abstractions.Services;
using Riber.Application.Common;

namespace Riber.Application.Features.Auths.Queries.GetAuthenticatedUser;

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