using Riber.Application.Abstractions.Commands;
using Riber.Application.Abstractions.Services;
using Riber.Application.Common;
using Riber.Application.Exceptions;
using Riber.Domain.Constants.Messages.Common;

namespace Riber.Application.Features.Auths.Commands.Login;

internal sealed class LoginCommandHandler(
    IAuthService authService,
    ITokenService tokenService)
    : ICommandHandler<LoginCommand, LoginCommandResponse>
{
    public async ValueTask<Result<LoginCommandResponse>> Handle(LoginCommand command,
        CancellationToken cancellationToken)
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
}