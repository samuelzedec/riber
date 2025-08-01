using SnackFlow.Application.Abstractions.Commands;
using SnackFlow.Domain.Enums;

namespace SnackFlow.Application.Features.Users.Commands.CreateUser;

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