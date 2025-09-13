using Riber.Application.Abstractions.Commands;

namespace Riber.Application.Features.Auths.Commands.Login;

public sealed record LoginCommandResponse(
    Guid UserApplicationId,
    Guid UserDomainId,
    string Token,
    string RefreshToken
) : ICommandResponse;