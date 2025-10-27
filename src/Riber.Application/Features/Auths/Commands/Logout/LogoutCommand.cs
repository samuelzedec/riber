using Riber.Application.Abstractions.Commands;
using Riber.Application.Common;

namespace Riber.Application.Features.Auths.Commands.Logout;

public sealed record LogoutCommand : ICommand<EmptyResult>;