using Riber.Domain.Entities;

namespace Riber.Application.Models;

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
    User UserDomain
);