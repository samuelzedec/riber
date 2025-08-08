using SnackFlow.Application.Abstractions.Queries;
using SnackFlow.Application.Common;
using SnackFlow.Application.Exceptions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Repositories;

namespace SnackFlow.Application.Features.Companies.Queries.GetCompanyById;

internal sealed class GetCompanyByIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetCompanyByIdQuery, GetCompanyByIdQueryResponse>
{
    public async ValueTask<Result<GetCompanyByIdQueryResponse>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var companyRepository = unitOfWork.Companies;
        var company = await companyRepository.GetSingleAsync(
            x => x.Id == request.CompanyId,
            cancellationToken
        );

        return company is null
            ? throw new NotFoundException(ErrorMessage.NotFound.Company)
            : GetCompanyByIdQueryResponse.FromCompany(company);
    }
}