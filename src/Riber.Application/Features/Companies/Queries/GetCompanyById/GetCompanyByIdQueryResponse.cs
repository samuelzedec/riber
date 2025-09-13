using Riber.Application.Abstractions.Queries;

namespace Riber.Application.Features.Companies.Queries.GetCompanyById;

public sealed record GetCompanyByIdQueryResponse(
    string CorporateName,
    string FantasyName,
    string Email,
    string Phone,
    string TaxId,
    string Type
) : IQueryResponse;