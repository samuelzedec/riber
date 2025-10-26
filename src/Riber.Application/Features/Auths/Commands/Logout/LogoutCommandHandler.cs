using Riber.Application.Abstractions.Commands;
using Riber.Application.Abstractions.Services;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Common;

namespace Riber.Application.Features.Auths.Commands.Logout;

internal sealed class LogoutCommandHandler(
    IAuthenticationService authenticationService,
    ICurrentUserService currentUserService)
    : ICommandHandler<LogoutCommand, EmptyResult>
{
    public async ValueTask<Result<EmptyResult>> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId().ToString();
        return await authenticationService.RefreshSecurityStampAsync(userId);
    }
}