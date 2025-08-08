using SnackFlow.Application.Abstractions.Commands;
using SnackFlow.Application.Extensions;
using SnackFlow.Domain.Entities;

namespace SnackFlow.Application.Features.Companies.Commands.CreateCompany;

public sealed record CreateCompanyCommandResponse(
    string FantasyName,
    string Email,
    string Phone,
    string Type,
    string PublicToken
) : ICommandResponse
{
    public static CreateCompanyCommandResponse FromCompany(Company company)
        => new(
            company.Name,
            company.Email,
            company.Phone,
            company.TaxId.Type.GetDescription(),
            company.PublicToken
        );
}