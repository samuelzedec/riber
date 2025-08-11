using SnackFlow.Application.Abstractions.Commands;

namespace SnackFlow.Application.Features.Auths.Commands.Login;

public sealed record LoginCommand(string EmailOrUserName, string Password ) 
    : ICommand<LoginCommandResponse>;