namespace Riber.Application.Models.User;

public sealed record CreateApplicationUserModel(
    string UserName,
    string Name,
    string Email,
    string PhoneNumber,
    string Password,
    Guid UserDomainId,
    ICollection<string> Roles
);