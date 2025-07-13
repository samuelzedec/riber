using SnackFlow.Application.Abstractions.Commands;

namespace SnackFlow.Application.Features.Companies.Commands.UpdateCompany;

public sealed record UpdateCompanyCommand(
    Guid CompanyId,
    string Email,
    string Phone,
    string FantasyName
) : ICommand<UpdateCompanyCommandResponse>;