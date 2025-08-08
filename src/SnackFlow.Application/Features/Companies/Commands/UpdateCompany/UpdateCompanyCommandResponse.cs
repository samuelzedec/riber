using SnackFlow.Application.Abstractions.Commands;
using SnackFlow.Application.Extensions;
using SnackFlow.Domain.Entities;

namespace SnackFlow.Application.Features.Companies.Commands.UpdateCompany;

public sealed record UpdateCompanyCommandResponse(
    string FantasyName,
    string Email,
    string Phone,
    string Type
) : ICommandResponse
{
    public static UpdateCompanyCommandResponse FromCompany(Company company) 
        => new(
            company.Name,
            company.Email,
            company.Phone,
            company.TaxId.Type.GetDescription()
        );
}