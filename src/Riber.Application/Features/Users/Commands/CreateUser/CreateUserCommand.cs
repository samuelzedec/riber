using Riber.Application.Abstractions.Commands;
using Riber.Domain.Enums;

namespace Riber.Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string FullName,
    string TaxId,
    BusinessPosition Position,
    Guid? CompanyId,
    string UserName,
    string Password,
    string Email,
    string PhoneNumber
) : ICommand<CreateUserCommandResponse>;