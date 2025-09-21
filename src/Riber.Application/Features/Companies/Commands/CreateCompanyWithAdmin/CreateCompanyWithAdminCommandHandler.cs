using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Commands;
using Riber.Application.Abstractions.Services;
using Riber.Application.Common;
using Riber.Application.DTOs;
using Riber.Application.Exceptions;
using Riber.Application.Extensions;
using Riber.Domain.Constants;
using Riber.Domain.Entities;
using Riber.Domain.Enums;
using Riber.Domain.Events;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.Company;
using Riber.Domain.Specifications.Core;
using Riber.Domain.ValueObjects.Email;
using Riber.Domain.ValueObjects.Phone;

namespace Riber.Application.Features.Companies.Commands.CreateCompanyWithAdmin;

internal sealed class CreateCompanyWithAdminCommandHandler(
    IUnitOfWork unitOfWork,
    IUserCreationService userCreationService,
    ILogger<CreateCompanyWithAdminCommandHandler> logger
    ) : ICommandHandler<CreateCompanyWithAdminCommand, CreateCompanyWithAdminCommandResponse>
{
    public async ValueTask<Result<CreateCompanyWithAdminCommandResponse>> Handle(
        CreateCompanyWithAdminCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await ValidateFieldsRequestAsync(request, cancellationToken);
            var companyEntity = Company.Create(
                corporateName: request.CorporateName,
                fantasyName: request.FantasyName,
                taxId: request.TaxId,
                email: request.Email,
                phone: request.Phone,
                type: request.Type
            );

            companyEntity.RaiseEvent(new CompanyWelcomeEmailRequestedEvent(
                companyEntity.Name,
                companyEntity.Email
            ));

            await unitOfWork.Companies.CreateAsync(companyEntity, cancellationToken);
            await userCreationService.CreateCompleteUserAsync(
                new CreateUserCompleteDTO(
                    FullName: request.AdminFullName,
                    UserName: request.AdminUserName,
                    Email: request.AdminEmail,
                    Password: request.AdminPassword,
                    PhoneNumber: request.AdminPhoneNumber,
                    TaxId: request.AdminTaxId,
                    Position: BusinessPosition.Owner,
                    Roles: ["Admin"],
                    CompanyId: companyEntity.Id
                ),
                cancellationToken
            );
            
            await unitOfWork.CommitTransactionAsync(cancellationToken);
            return new CreateCompanyWithAdminCommandResponse(
                CompanyId: companyEntity.Id,
                FantasyName: companyEntity.Name,
                Email: companyEntity.Email,
                Phone: companyEntity.Phone,
                Type: companyEntity.TaxId.Type.GetDescription(),
                AdminUserEmail: request.AdminEmail,
                AdminUserName: request.AdminUserName
            );
        }
        catch (ConflictException)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            logger.LogError(ex, ErrorMessage.Exception.Unexpected(ex.GetType().Name, ex.Message));
            throw;
        }
    }

    private async Task ValidateFieldsRequestAsync(
        CreateCompanyWithAdminCommand request,
        CancellationToken cancellationToken)
    {
        var companyRepository = unitOfWork.Companies;
        var normalizedPhone = Phone.RemoveFormatting(request.Phone);
        var normalizedEmail = Email.Standardization(request.Email);
        var normalizedTaxId = new string([..request.TaxId.Where(char.IsDigit)]);
        
        var validations = new (Specification<Company>, string message)[]
        {
            (new CorporateNameSpecification(request.CorporateName), ErrorMessage.Conflict.CorporateNameAlreadyExists),
            (new CompanyTaxIdSpecification(normalizedTaxId), ErrorMessage.Conflict.TaxIdAlreadyExists),
            (new CompanyEmailSpecification(normalizedEmail), ErrorMessage.Conflict.EmailAlreadyExists),
            (new CompanyPhoneSpecification(normalizedPhone), ErrorMessage.Conflict.PhoneAlreadyExists)
        };

        foreach ((Specification<Company> expression, string message) in validations)
        {
            if (await companyRepository.ExistsAsync(expression, cancellationToken))
                throw new ConflictException(message);
        }
    }
}