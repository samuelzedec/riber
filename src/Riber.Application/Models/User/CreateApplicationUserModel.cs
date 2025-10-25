namespace Riber.Application.Models.Shared;

public sealed record CreateApplicationUserModel(
    string UserName,
    string Name,
    string Email,
    string PhoneNumber,
    string Password,
    Guid UserDomainId,
    ICollection<string> Roles
);