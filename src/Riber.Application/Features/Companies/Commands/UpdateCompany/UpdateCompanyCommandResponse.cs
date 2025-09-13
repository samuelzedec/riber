using Riber.Application.Abstractions.Commands;

namespace Riber.Application.Features.Companies.Commands.UpdateCompany;

public sealed record UpdateCompanyCommandResponse(
    string FantasyName,
    string Email,
    string Phone,
    string Type
) : ICommandResponse;