namespace Riber.Application.Dtos.User;

public sealed record CreateApplicationUserDto(
    string UserName,
    string Name,
    string Email,
    string PhoneNumber,
    string Password,
    Guid UserDomainId,
    ICollection<string> Roles
);