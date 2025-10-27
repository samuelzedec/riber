using Riber.Application.Abstractions.Commands;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Common;
using Riber.Domain.Constants.Messages.Common;

namespace Riber.Application.Features.Auths.Commands.Login;

internal sealed class LoginCommandHandler(
    IAuthenticationService authenticationService,
    ITokenService tokenService)
    : ICommandHandler<LoginCommand, LoginCommandResponse>
{
    public async ValueTask<Result<LoginCommandResponse>> Handle(LoginCommand command,
        CancellationToken cancellationToken)
    {
        var userDetailsModel = await authenticationService.LoginAsync(command.EmailOrUserName, command.Password);
        if (userDetailsModel is null)
            return Result.Failure<LoginCommandResponse>(AuthenticationErrors.InvalidCredentials);

        var token = tokenService.GenerateToken(userDetailsModel);
        var refreshToken = tokenService.GenerateRefreshToken(userDetailsModel.Id, userDetailsModel.SecurityStamp);

        return new LoginCommandResponse(
            UserApplicationId: userDetailsModel.Id,
            UserDomainId: userDetailsModel.UserDomainId,
            Token: token,
            RefreshToken: refreshToken
        );
    }
}