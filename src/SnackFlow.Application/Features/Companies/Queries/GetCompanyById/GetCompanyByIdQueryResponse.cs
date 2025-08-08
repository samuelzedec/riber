using SnackFlow.Application.Abstractions.Queries;
using SnackFlow.Application.Extensions;
using SnackFlow.Domain.Entities;

namespace SnackFlow.Application.Features.Companies.Queries.GetCompanyById;

public sealed record GetCompanyByIdQueryResponse(
    string CorporateName,
    string FantasyName,
    string Email,
    string Phone,
    string TaxId,
    string Type,
    string PublicToken
) : IQueryResponse
{
    public static GetCompanyByIdQueryResponse FromCompany(Company company) 
        => new(
            company.Name.Corporate,
            company.Name,
            company.Email,
            company.Phone,
            company.TaxId,
            company.TaxId.Type.GetDescription(),
            company.PublicToken
        );
}