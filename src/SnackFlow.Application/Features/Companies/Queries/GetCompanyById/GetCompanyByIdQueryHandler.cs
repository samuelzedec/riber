using SnackFlow.Application.Abstractions.Queries;
using SnackFlow.Application.Common;
using SnackFlow.Application.Exceptions;
using SnackFlow.Application.Extensions;
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

        if (company is null)
            throw new NotFoundException(ErrorMessage.NotFound.Company);

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