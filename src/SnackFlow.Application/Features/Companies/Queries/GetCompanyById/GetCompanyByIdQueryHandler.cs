using SnackFlow.Application.Abstractions.Queries;
using SnackFlow.Application.Common;
using SnackFlow.Application.Exceptions;
using SnackFlow.Application.Extensions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Repositories;

namespace SnackFlow.Application.Features.Companies.Queries.GetCompanyById;

public sealed class GetCompanyByIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetCompanyByIdQuery, GetCompanyByIdQueryResponse>
{
    public async Task<Result<GetCompanyByIdQueryResponse>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var companyRespository = unitOfWork.Companies;
        var company = await companyRespository.GetSingleAsync(
            x => x.Id == request.CompanyId,
            cancellationToken
        );

        if (company is null)
            throw new NotFoundException(ErrorMessage.NotFound.Company);

        return new GetCompanyByIdQueryResponse(
            Name: company.CompanyName.Name,
            TradingName: company.CompanyName,
            Email: company.Email,
            Phone: company.Phone,
            TaxId: company.TaxId,
            Type: company.TaxId.Type.GetDescription()
        );
    }
}