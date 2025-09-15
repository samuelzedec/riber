using Riber.Domain.Entities;

namespace Riber.Application.DTOs;

public sealed record UserDetailsDTO(
    Guid Id,
    string UserName,
    string Email,
    bool EmailConfirmed,
    string PhoneNumber,
    string SecurityStamp,
    ICollection<string> Roles,
    ICollection<ClaimDTO> Claims,
    Guid UserDomainId,
    User UserDomain
);