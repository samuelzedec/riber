using System.Linq.Expressions;
using SnackFlow.Application.Abstractions.Commands;
using SnackFlow.Application.Common;
using SnackFlow.Application.Exceptions;
using SnackFlow.Application.Extensions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Entities;
using SnackFlow.Domain.Repositories;
using SnackFlow.Domain.ValueObjects.Email;
using SnackFlow.Domain.ValueObjects.Phone;

namespace SnackFlow.Application.Features.Companies.Commands.CreateCompany;

public sealed class CreateCompanyCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateCompanyCommand, CreateCompanyCommandResponse>
{
    public async Task<Result<CreateCompanyCommandResponse>> Handle(
        CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var companyRepository = unitOfWork.Companies;
        await ValidateFieldsRequestAsync(request, cancellationToken);

        var companyEntity = Company.Create(
            name: request.Name,
            tradingName: request.TradingName,
            taxId: request.TaxId,
            email: request.Email,
            phone: request.Phone,
            type: request.Type
        );

        await companyRepository.CreateAsync(companyEntity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateCompanyCommandResponse(
            CompanyId: companyEntity.Id,
            TradingName: companyEntity.Name,
            Email: companyEntity.Email,
            Phone: companyEntity.Phone,
            Type: companyEntity.TaxId.Type.GetDescription()
        );
    }

    private async Task ValidateFieldsRequestAsync(
        CreateCompanyCommand request,
        CancellationToken cancellationToken)
    {
        var companyRepository = unitOfWork.Companies;
        var normalizedPhone = Phone.RemoveFormatting(request.Phone);
        var normalizedEmail = Email.Standardization(request.Email);
        
        var validations = new (Expression<Func<Company, bool>>, string message)[]
        {
            (x => x.Name.Corporate == request.Name, ErrorMessage.Conflict.CorporateNameAlreadyExists),
            (x => x.TaxId.Value == request.TaxId, ErrorMessage.Conflict.TaxIdAlreadyExists),
            (x => x.Email.Value == normalizedEmail, ErrorMessage.Conflict.EmailAlreadyExists),
            (x => x.Phone.Value == normalizedPhone, ErrorMessage.Conflict.PhoneAlreadyExists)
        };

        foreach (var (expression, message) in validations)
        {
            if (await companyRepository.ExistsAsync(expression, cancellationToken))
                throw new ConflictException(message);
        }
    }
}