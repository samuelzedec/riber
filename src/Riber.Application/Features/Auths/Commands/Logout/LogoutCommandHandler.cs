using Riber.Application.Abstractions.Commands;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Common;
using Riber.Domain.Constants.Messages.Common;

namespace Riber.Application.Features.Auths.Commands.Logout;

internal sealed class LogoutCommandHandler(
    IAuthenticationService authenticationService,
    ICurrentUserService currentUserService)
    : ICommandHandler<LogoutCommand, EmptyResult>
{
    public async ValueTask<Result<EmptyResult>> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId().ToString();
        var result = await authenticationService.RefreshSecurityStampAsync(userId);
        return result ? Result.Success() : Result.Failure(AuthenticationErrors.InvalidCredentials);
    }
}