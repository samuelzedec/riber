using Microsoft.Extensions.Logging;
using SnackFlow.Application.Abstractions.Commands;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.Common;
using SnackFlow.Application.Exceptions;
using SnackFlow.Domain.Constants;

namespace SnackFlow.Application.Features.Auths.Commands.Login;

public sealed class LoginCommandHandler(
    IAuthService authService,
    ITokenService tokenService,
    ILogger<LoginCommandHandler> logger)
    : ICommandHandler<LoginCommand, LoginCommandResponse>
{
    public async ValueTask<Result<LoginCommandResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var user = await authService.LoginAsync(command.EmailOrUserName, command.Password)
                ?? throw new UnauthorizedException(ErrorMessage.Invalid.Password);
            
            var token = tokenService.GenerateToken(user);
            var refreshToken = tokenService.GenerateRefreshToken(user.Id, user.SecurityStamp);

            return new LoginCommandResponse(
                UserApplicationId: user.Id,
                UserDomainId: user.UserDomainId,
                Token: token,
                RefreshToken: refreshToken
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessage.Exception.Unexpected(ex.GetType().Name, ex.Message));
            throw;
        }
    }
}