using SnackFlow.Domain.Enums;

namespace SnackFlow.Application.DTOs;

public sealed record CreateUserCompleteDTO(
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