using Riber.Application.Abstractions.Commands;

namespace Riber.Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommandResponse(
    string UserName,
    string Email
) : ICommandResponse;