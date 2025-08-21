using Microsoft.Extensions.Logging;
using SnackFlow.Application.Abstractions.Commands;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.Common;
using SnackFlow.Application.Exceptions;
using SnackFlow.Domain.Constants;

namespace SnackFlow.Application.Features.Auths.Commands.Logout;

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
            var userId = currentUserService.GetUserId()?.ToString()
                ?? throw new BadRequestException(ErrorMessage.Invalid.IdIsNull);

            await authService.UpdateSecurityStampAndGetUserAsync(userId);
            return Result.Success();
        }
        catch (Exception ex) when(ex is not BadRequestException)
        {
            logger.LogError(ex, ErrorMessage.Exception.Unexpected(ex.GetType().Name, ex.Message));
            throw;
        }
    }
}