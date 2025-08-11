using SnackFlow.Application.Abstractions.Commands;
using SnackFlow.Domain.Enums;

namespace SnackFlow.Application.Features.Companies.Commands.CreateCompanyWithAdmin;

public sealed record CreateCompanyWithAdminCommand(
    string CorporateName,
    string FantasyName,
    string TaxId,
    string Email,
    string Phone,
    TaxIdType Type,
    string AdminFullName,
    string AdminTaxId,
    string AdminUserName,
    string AdminPassword,
    string AdminEmail,
    string AdminPhoneNumber
) : ICommand<CreateCompanyWithAdminCommandResponse>;