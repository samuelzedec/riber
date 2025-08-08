using SnackFlow.Application.Abstractions.Commands;
using SnackFlow.Application.DTOs;
using SnackFlow.Domain.Entities;
using SnackFlow.Domain.Enums;
using EmailVo = SnackFlow.Domain.ValueObjects.Email.Email;
using PhoneVo = SnackFlow.Domain.ValueObjects.Phone.Phone;

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
) : ICommand<CreateUserCommandResponse>
{
    public static CreateApplicationUserDTO ToApplicationUserDto(CreateUserCommand command, IEnumerable<string> permissions)
        => new()
        {
            UserName = command.UserName,
            Email = EmailVo.Create(command.Email).Value,
            Name = command.FullName.Split(' ')[0],
            Password = command.Password,
            PhoneNumber = PhoneVo.Create(command.PhoneNumber).Value,
            Roles = ["User"],
            Permissions = permissions,
        };

    public static User ToDomainUser(CreateUserCommand command, Guid applicationUserId) 
        => User.Create(
            fullName: command.FullName,
            taxId: command.TaxId,
            applicationUserId: applicationUserId,
            position: command.Position,
            companyId: command.CompanyId
        );
}