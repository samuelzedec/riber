using SnackFlow.Application.Abstractions.Queries;

namespace SnackFlow.Application.Features.Companies.Queries.GetCompanyById;

public sealed record GetCompanyByIdQueryResponse(
    string Name,
    string TradingName,
    string Email,
    string Phone,
    string TaxId,
    string Type
) : IQueryResponse;