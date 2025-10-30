using Riber.Domain.Enums;

namespace Riber.Application.Dtos.User;

public sealed record CreateUserCompleteDto(
    string FullName,
    string UserName,
    string Email,
    string Password,
    string PhoneNumber,
    string TaxId,
    BusinessPosition Position,
    List<string> Roles,
    Guid? CompanyId = null
);