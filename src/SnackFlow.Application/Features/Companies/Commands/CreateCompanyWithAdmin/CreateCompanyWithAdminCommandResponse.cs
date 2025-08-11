using SnackFlow.Application.Abstractions.Commands;

namespace SnackFlow.Application.Features.Companies.Commands.CreateCompanyWithAdmin;

public sealed record CreateCompanyWithAdminCommandResponse(
    Guid CompanyId,
    string FantasyName,
    string Email,
    string Phone,
    string Type,
    string AdminUserName,
    string AdminUserEmail
) : ICommandResponse;