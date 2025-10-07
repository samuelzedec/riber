using Riber.Application.Abstractions.Commands;
using Riber.Application.Abstractions.Services;
using Riber.Application.Common;

namespace Riber.Application.Features.Auths.Commands.Logout;

internal sealed class LogoutCommandHandler(
    IAuthService authService,
    ICurrentUserService currentUserService)
    : ICommandHandler<LogoutCommand>
{
    public async ValueTask<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId().ToString();
        await authService.RefreshUserSecurityAsync(userId);
        return Result.Success();
    }
}