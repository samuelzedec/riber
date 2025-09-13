using Riber.Application.Abstractions.Commands;

namespace Riber.Application.Features.Companies.Commands.UpdateCompany;

public sealed record UpdateCompanyCommand(
    Guid CompanyId,
    string Email,
    string Phone,
    string FantasyName
) : ICommand<UpdateCompanyCommandResponse>;