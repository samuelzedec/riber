using Riber.Application.Abstractions.Commands;

namespace Riber.Application.Features.Auths.Commands.Login;

public sealed record LoginCommand(string EmailOrUserName, string Password ) 
    : ICommand<LoginCommandResponse>;