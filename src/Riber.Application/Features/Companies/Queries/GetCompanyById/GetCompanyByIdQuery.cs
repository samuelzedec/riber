using Riber.Application.Abstractions.Queries;

namespace Riber.Application.Features.Companies.Queries.GetCompanyById;

public sealed record GetCompanyByIdQuery(Guid CompanyId) 
    : IQuery<GetCompanyByIdQueryResponse>;