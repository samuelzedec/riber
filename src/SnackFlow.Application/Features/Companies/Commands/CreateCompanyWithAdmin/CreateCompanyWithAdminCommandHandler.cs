using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using SnackFlow.Application.Abstractions.Commands;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.Common;
using SnackFlow.Application.DTOs;
using SnackFlow.Application.Exceptions;
using SnackFlow.Application.Extensions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Entities;
using SnackFlow.Domain.Enums;
using SnackFlow.Domain.Events;
using SnackFlow.Domain.Repositories;
using SnackFlow.Domain.ValueObjects.Email;
using SnackFlow.Domain.ValueObjects.Phone;

namespace SnackFlow.Application.Features.Companies.Commands.CreateCompanyWithAdmin;

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
        
        var validations = new (Expression<Func<Company, bool>>, string message)[]
        {
            (x => x.Name.Corporate == request.CorporateName, ErrorMessage.Conflict.CorporateNameAlreadyExists),
            (x => x.TaxId.Value == request.TaxId, ErrorMessage.Conflict.TaxIdAlreadyExists),
            (x => x.Email.Value == normalizedEmail, ErrorMessage.Conflict.EmailAlreadyExists),
            (x => x.Phone.Value == normalizedPhone, ErrorMessage.Conflict.PhoneAlreadyExists)
        };

        foreach ((Expression<Func<Company, bool>> expression, string message) in validations)
        {
            if (await companyRepository.ExistsAsync(expression, cancellationToken))
                throw new ConflictException(message);
        }
    }
}