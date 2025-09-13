namespace Riber.Application.DTOs;

public sealed record CreateApplicationUserDTO(
    string UserName,
    string Name,
    string Email,
    string PhoneNumber,
    string Password,
    Guid UserDomainId,
    ICollection<string> Roles
);