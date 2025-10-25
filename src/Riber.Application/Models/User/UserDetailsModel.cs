using Riber.Application.Models.Auth;

namespace Riber.Application.Models.User;

public sealed record UserDetailsModel(
    Guid Id,
    string UserName,
    string Email,
    bool EmailConfirmed,
    string PhoneNumber,
    string SecurityStamp,
    ICollection<string> Roles,
    ICollection<ClaimModel> Claims,
    Guid UserDomainId,
    Domain.Entities.User UserDomain
);