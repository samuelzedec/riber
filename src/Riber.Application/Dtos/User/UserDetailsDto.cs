using Riber.Application.Dtos.Auth;

namespace Riber.Application.Dtos.User;

public sealed record UserDetailsDto(
    Guid Id,
    string UserName,
    string Email,
    bool EmailConfirmed,
    string PhoneNumber,
    string SecurityStamp,
    ICollection<string> Roles,
    ICollection<ClaimDto> Claims,
    Guid UserDomainId,
    Domain.Entities.User UserDomain
);