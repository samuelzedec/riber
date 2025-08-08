using SnackFlow.Application.Abstractions.Commands;

namespace SnackFlow.Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommandResponse(
    string UserName,
    string Email,
    string PublicToken
) : ICommandResponse;