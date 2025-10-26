using Riber.Application.Abstractions.Commands;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Common;

namespace Riber.Application.Features.Auths.Commands.Login;

internal sealed class LoginCommandHandler(
    IAuthenticationService authenticationService,
    ITokenService tokenService)
    : ICommandHandler<LoginCommand, LoginCommandResponse>
{
    public async ValueTask<Result<LoginCommandResponse>> Handle(LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await authenticationService.LoginAsync(command.EmailOrUserName, command.Password);
        if (!result.IsSuccess)
            return Result.Failure<LoginCommandResponse>(result.Error.Message, result.StatusCode);

        var user = result.Value!;
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