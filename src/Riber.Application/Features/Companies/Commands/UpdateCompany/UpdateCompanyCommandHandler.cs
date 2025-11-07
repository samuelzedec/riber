using System.Net;
using Riber.Application.Abstractions.Commands;
using Riber.Application.Common;
using Riber.Application.Extensions;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Entities;
using Riber.Domain.Entities.Company;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.Company;
using Riber.Domain.Specifications.Core;
using Riber.Domain.ValueObjects.Email;
using Riber.Domain.ValueObjects.Phone;

namespace Riber.Application.Features.Companies.Commands.UpdateCompany;

internal sealed class UpdateCompanyCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateCompanyCommand, UpdateCompanyCommandResponse>
{
    public async ValueTask<Result<UpdateCompanyCommandResponse>> Handle(UpdateCompanyCommand request,
        CancellationToken cancellationToken)
    {
        var companyRepository = unitOfWork.Companies;
        var company = await companyRepository
            .GetSingleAsync(new CompanyIdSpecification(request.CompanyId), cancellationToken);

        if (company is null)
            return Result.Failure<UpdateCompanyCommandResponse>(NotFoundErrors.Company, HttpStatusCode.NotFound);

        var validationResult = await ValidateAsync(
            company,
            request.Email,
            request.Phone,
            request.FantasyName,
            cancellationToken
        );

        if (!validationResult.IsSuccess)
            return validationResult;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new UpdateCompanyCommandResponse(
            company.Name,
            company.Email,
            company.Phone,
            company.TaxId.Type.GetDescription()
        );
    }

    private async Task<Result<UpdateCompanyCommandResponse>> ValidateAsync(
        Company company,
        string email,
        string phone,
        string fantasyName,
        CancellationToken cancellationToken = default)
    {
        var updateEmailMessage = await UpdateEmailAsync(company, email, cancellationToken);
        if (updateEmailMessage is not null)
            return Result.Failure<UpdateCompanyCommandResponse>(updateEmailMessage, HttpStatusCode.Conflict);

        var updatePhoneMessage = await UpdatePhoneAsync(company, phone, cancellationToken);
        if (updatePhoneMessage is not null)
            return Result.Failure<UpdateCompanyCommandResponse>(updatePhoneMessage, HttpStatusCode.Conflict);

        UpdateFantasyName(company, fantasyName);
        return Result.Success<UpdateCompanyCommandResponse>();
    }

    private async Task<string?> UpdateEmailAsync(Company company, string email,
        CancellationToken cancellationToken = default)
    {
        email = Email.Standardization(email);
        if (string.IsNullOrWhiteSpace(email) || company.Email.Value == email)
            return null;

        if (await CheckForConflictAsync(new CompanyEmailSpecification(email), cancellationToken))
            return ConflictErrors.Email;

        company.UpdateEmail(email);
        return null;
    }

    private async Task<string?> UpdatePhoneAsync(
        Company company,
        string phone,
        CancellationToken cancellationToken = default)
    {
        phone = Phone.RemoveFormatting(phone);
        if (string.IsNullOrWhiteSpace(phone) || company.Phone.Value == phone)
            return null;

        if (await CheckForConflictAsync(new CompanyPhoneSpecification(phone), cancellationToken))
            return ConflictErrors.Phone;

        company.UpdatePhone(phone);
        return null;
    }

    private static void UpdateFantasyName(Company company, string fantasyName)
    {
        if (string.IsNullOrWhiteSpace(fantasyName) || fantasyName.Equals(company.Name))
            return;

        company.UpdateFantasyName(fantasyName);
    }

    private async Task<bool> CheckForConflictAsync(
        Specification<Company> specification,
        CancellationToken cancellationToken = default)
    {
        var companyRepository = unitOfWork.Companies;
        return await companyRepository.ExistsAsync(specification, cancellationToken);
    }
}