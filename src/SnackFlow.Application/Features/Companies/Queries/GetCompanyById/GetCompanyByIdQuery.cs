using SnackFlow.Application.Abstractions.Queries;

namespace SnackFlow.Application.Features.Companies.Queries.GetCompanyById;

public sealed record GetCompanyByIdQuery(Guid CompanyId) 
    : IQuery<GetCompanyByIdQueryResponse>;