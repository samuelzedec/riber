using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Commands;
using Riber.Application.Abstractions.Services;
using Riber.Application.Common;
using Riber.Application.Exceptions;
using Riber.Domain.Constants.Messages.Common;

namespace Riber.Application.Features.Auths.Commands.Login;

internal sealed class LoginCommandHandler(
    IAuthService authService,
    ITokenService tokenService,
    ILogger<LoginCommandHandler> logger)
    : ICommandHandler<LoginCommand, LoginCommandResponse>
{
    public async ValueTask<Result<LoginCommandResponse>> Handle(LoginCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await authService.LoginAsync(command.EmailOrUserName, command.Password)
                       ?? throw new UnauthorizedException(PasswordErrors.Invalid);

            var token = tokenService.GenerateToken(user);
            var refreshToken = tokenService.GenerateRefreshToken(user.Id, user.SecurityStamp);

            return new LoginCommandResponse(
                UserApplicationId: user.Id,
                UserDomainId: user.UserDomainId,
                Token: token,
                RefreshToken: refreshToken
            );
        }
        catch (Exception ex) when (ex is not UnauthorizedException)
        {
            logger.LogError(ex,
                "[{ClassName}] exceção inesperada: {ExceptionType} - {ExceptionMessage}",
                nameof(LoginCommandHandler),
                ex.GetType(),
                ex.Message);
            throw;
        }
    }
}