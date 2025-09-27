using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Commands;
using Riber.Application.Abstractions.Services;
using Riber.Application.Common;
using Riber.Application.Exceptions;
using Riber.Domain.Constants;

namespace Riber.Application.Features.Auths.Commands.Logout;

internal sealed class LogoutCommandHandler(
    IAuthService authService,
    ICurrentUserService currentUserService,
    ILogger<LogoutCommandHandler> logger)
    : ICommandHandler<LogoutCommand>
{
    public async ValueTask<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var userId = currentUserService.GetUserId().ToString();
            await authService.RefreshUserSecurityAsync(userId);
            return Result.Success();
        }
        catch (Exception ex) when(ex is not BadRequestException)
        {
            logger.LogError(ex, ErrorMessage.Exception.Unexpected(ex.GetType().Name, ex.Message));
            throw;
        }
    }
}