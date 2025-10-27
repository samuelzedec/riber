using Riber.Domain.Enums;

namespace Riber.Application.Models.User;

public sealed record CreateUserCompleteModel(
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