using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Commands;
using Riber.Application.Common;
using Riber.Application.Exceptions;
using Riber.Application.Extensions;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Entities;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.Company;
using Riber.Domain.Specifications.Core;
using Riber.Domain.ValueObjects.Email;
using Riber.Domain.ValueObjects.Phone;

namespace Riber.Application.Features.Companies.Commands.UpdateCompany;

internal sealed class UpdateCompanyCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdateCompanyCommandHandler> logger)
    : ICommandHandler<UpdateCompanyCommand, UpdateCompanyCommandResponse>
{
    public async ValueTask<Result<UpdateCompanyCommandResponse>> Handle(UpdateCompanyCommand request,
        CancellationToken cancellationToken)
    {
        var companyRepository = unitOfWork.Companies;
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            var company = await companyRepository.GetSingleAsync(
                new CompanyIdSpecification(request.CompanyId), cancellationToken)
                ?? throw new NotFoundException(NotFoundErrors.Company);

            await UpdateEmailAsync(company, request.Email, cancellationToken);
            await UpdatePhoneAsync(company, request.Phone, cancellationToken);
            UpdateFantasyName(company, request.FantasyName);

            companyRepository.Update(company);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return new UpdateCompanyCommandResponse(
                company.Name,
                company.Email,
                company.Phone,
                company.TaxId.Type.GetDescription()
            );
        }
        catch (Exception ex)
        {
            logger.LogError(UnexpectedErrors.ForLogging(nameof(UpdateCompanyCommandHandler), ex));
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task UpdateEmailAsync(Company company, string email, CancellationToken cancellationToken = default)
    {
        email = Email.Standardization(email);
        if (string.IsNullOrWhiteSpace(email) || company.Email.Value == email)
            return;

        await CheckForConflictAsync(
            new CompanyEmailSpecification(email),
            ConflictErrors.Email,
            cancellationToken
        );

        company.UpdateEmail(email);
    }

    private async Task UpdatePhoneAsync(Company company, string phone, CancellationToken cancellationToken = default)
    {
        phone = Phone.RemoveFormatting(phone);
        if (string.IsNullOrWhiteSpace(phone) || company.Phone.Value == phone)
            return;

        await CheckForConflictAsync(
            new CompanyPhoneSpecification(phone),
            ConflictErrors.Phone,
            cancellationToken
        );

        company.UpdatePhone(phone);
    }

    private void UpdateFantasyName(Company company, string fantasyName)
    {
        if (string.IsNullOrWhiteSpace(fantasyName) || company.Name == fantasyName)
            return;

        company.UpdateFantasyName(fantasyName);
    }

    private async Task CheckForConflictAsync(
        Specification<Company> specification,
        string message,
        CancellationToken cancellationToken = default)
    {
        var companyRepository = unitOfWork.Companies;
        if (await companyRepository.ExistsAsync(specification, cancellationToken))
            throw new ConflictException(message);
    }
}