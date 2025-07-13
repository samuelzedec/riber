using SnackFlow.Application.Abstractions.Commands;

namespace SnackFlow.Application.Features.Companies.Commands.CreateCompany;

public sealed record CreateCompanyCommandResponse(
    Guid CompanyId,
    string FantasyName,
    string Email,
    string Phone,
    string Type
) : ICommandResponse;