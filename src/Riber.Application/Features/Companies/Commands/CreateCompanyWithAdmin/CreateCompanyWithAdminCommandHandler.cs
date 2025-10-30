using System.Net;
using Riber.Application.Abstractions.Commands;
using Riber.Application.Abstractions.Services;
using Riber.Application.Common;
using Riber.Application.Dtos.User;
using Riber.Application.Extensions;
using Riber.Domain.Constants.Messages.Common;
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
    IUserCreationService userCreationService)
    : ICommandHandler<CreateCompanyWithAdminCommand, CreateCompanyWithAdminCommandResponse>
{
    public async ValueTask<Result<CreateCompanyWithAdminCommandResponse>> Handle(
        CreateCompanyWithAdminCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var validationMessage = await ValidateFieldsRequestAsync(request, cancellationToken);
            if (!validationMessage.IsSuccess) return validationMessage;

            var companyEntity = Company.Create(
                corporateName: request.CorporateName,
                fantasyName: request.FantasyName,
                taxId: request.TaxId,
                email: request.Email,
                phone: request.Phone,
                type: request.Type
            );

            companyEntity.RaiseEvent(new CompanyWelcomeEmailRequestedEvent(companyEntity.Name, companyEntity.Email));
            var createUserResult = await CreateUserAsync(companyEntity, request, cancellationToken);
            if (!createUserResult.IsSuccess) return createUserResult;

            await unitOfWork.CommitTransactionAsync(cancellationToken);
            return Result.Success(new CreateCompanyWithAdminCommandResponse(
                CompanyId: companyEntity.Id,
                FantasyName: companyEntity.Name,
                Email: companyEntity.Email,
                Phone: companyEntity.Phone,
                Type: companyEntity.TaxId.Type.GetDescription(),
                AdminUserEmail: request.AdminEmail,
                AdminUserName: request.AdminUserName
            ), HttpStatusCode.Created);
        }
        finally
        {
            if (unitOfWork.HasActiveTransaction())
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
        }
    }

    private async Task<Result<CreateCompanyWithAdminCommandResponse>> CreateUserAsync(
        Company companyEntity,
        CreateCompanyWithAdminCommand request,
        CancellationToken cancellationToken)
    {
        await unitOfWork.Companies.CreateAsync(companyEntity, cancellationToken);
        var createUserDto = new CreateUserCompleteDto(
            FullName: request.AdminFullName,
            UserName: request.AdminUserName,
            Email: request.AdminEmail,
            Password: request.AdminPassword,
            PhoneNumber: request.AdminPhoneNumber,
            TaxId: request.AdminTaxId,
            Position: BusinessPosition.Owner,
            Roles: ["Admin"],
            CompanyId: companyEntity.Id
        );

        var userCreationResult = await userCreationService.CreateCompleteUserAsync(createUserDto, cancellationToken);
        return !userCreationResult.IsSuccess
            ? Result.Failure<CreateCompanyWithAdminCommandResponse>(userCreationResult.Error.Message,
                userCreationResult.StatusCode)
            : Result.Success<CreateCompanyWithAdminCommandResponse>();
    }

    private async Task<Result<CreateCompanyWithAdminCommandResponse>> ValidateFieldsRequestAsync(
        CreateCompanyWithAdminCommand request,
        CancellationToken cancellationToken)
    {
        var companyRepository = unitOfWork.Companies;
        var normalizedPhone = Phone.RemoveFormatting(request.Phone);
        var normalizedEmail = Email.Standardization(request.Email);
        var normalizedTaxId = new string([.. request.TaxId.Where(char.IsDigit)]);

        var validations = new (Specification<Company>, string message)[]
        {
            (new CorporateNameSpecification(request.CorporateName), ConflictErrors.CorporateName),
            (new CompanyTaxIdSpecification(normalizedTaxId), ConflictErrors.TaxId),
            (new CompanyEmailSpecification(normalizedEmail), ConflictErrors.Email),
            (new CompanyPhoneSpecification(normalizedPhone), ConflictErrors.Phone)
        };

        foreach ((Specification<Company> expression, string message) in validations)
        {
            if (await companyRepository.ExistsAsync(expression, cancellationToken))
                return Result.Failure<CreateCompanyWithAdminCommandResponse>(message, HttpStatusCode.Conflict);
        }

        return Result.Success<CreateCompanyWithAdminCommandResponse>();
    }
}