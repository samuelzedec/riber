using Riber.Application.Abstractions.Queries;
using Riber.Application.Common;
using Riber.Application.Exceptions;
using Riber.Application.Extensions;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.Company;

namespace Riber.Application.Features.Companies.Queries.GetCompanyById;

internal sealed class GetCompanyByIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetCompanyByIdQuery, GetCompanyByIdQueryResponse>
{
    public async ValueTask<Result<GetCompanyByIdQueryResponse>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var companyRepository = unitOfWork.Companies;
        var company = await companyRepository.GetSingleAsync(
            new CompanyIdSpecification(request.CompanyId),
            cancellationToken
        );

        if (company is null)
            throw new NotFoundException(NotFoundErrors.Company);

        return new GetCompanyByIdQueryResponse(
            CorporateName: company.Name.Corporate,
            FantasyName: company.Name,
            Email: company.Email,
            Phone: company.Phone,
            TaxId: company.TaxId,
            Type: company.TaxId.Type.GetDescription()
        );
    }
}