using SnackFlow.Application.Abstractions.Commands;

namespace SnackFlow.Application.Features.Auths.Commands.Login;

public sealed record LoginCommandResponse(
    Guid UserApplicationId,
    Guid UserDomainId,
    string Token,
    string RefreshToken
) : ICommandResponse;