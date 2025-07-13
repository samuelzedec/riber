using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using SnackFlow.Application.Abstractions.Commands;
using SnackFlow.Application.Common;
using SnackFlow.Application.Exceptions;
using SnackFlow.Application.Extensions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Entities;
using SnackFlow.Domain.Repositories;
using SnackFlow.Domain.ValueObjects.Email;
using SnackFlow.Domain.ValueObjects.Phone;

namespace SnackFlow.Application.Features.Companies.Commands.UpdateCompany;

public sealed class UpdateCompanyCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdateCompanyCommandHandler> logger)
    : ICommandHandler<UpdateCompanyCommand, UpdateCompanyCommandResponse>
{
    public async Task<Result<UpdateCompanyCommandResponse>> Handle(UpdateCompanyCommand request,
        CancellationToken cancellationToken)
    {
        var companyRepository = unitOfWork.Companies;
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            var company = await companyRepository.GetSingleAsync(
                x => x.Id == request.CompanyId, cancellationToken);

            if (company is null)
                throw new NotFoundException(ErrorMessage.NotFound.Company);

            await UpdateEmailAsync(company, request.Email, cancellationToken);
            await UpdatePhoneAsync(company, request.Phone, cancellationToken);
            UpdateTradingName(company, request.TradingName);

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
            logger.LogError(ex.Message, ex.StackTrace);
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task UpdateEmailAsync(Company company, string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email) || company.Email.Value == Email.Standardization(email))
            return;

        email = Email.Standardization(email);
        await CheckForConflictAsync(
            x => x.Email.Value == email,
            ErrorMessage.Conflict.EmailAlreadyExists,
            cancellationToken
        );

        company.UpdateEmail(email);
    }

    private async Task UpdatePhoneAsync(Company company, string phone, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phone) || company.Phone.Value == Phone.RemoveFormatting(phone))
            return;

        phone = Phone.RemoveFormatting(phone);
        await CheckForConflictAsync(
            x => x.Phone.Value == phone,
            ErrorMessage.Conflict.PhoneAlreadyExists,
            cancellationToken
        );

        company.UpdatePhone(phone);
    }

    private void UpdateTradingName(Company company, string tradingName)
    {
        if (string.IsNullOrWhiteSpace(tradingName) || company.Name == tradingName) 
            return;
        
        company.UpdateTradingName(tradingName);
    }

    private async Task CheckForConflictAsync(
        Expression<Func<Company, bool>> expression,
        string message,
        CancellationToken cancellationToken = default)
    {
        var companyRepository = unitOfWork.Companies;
        if (await companyRepository.ExistsAsync(expression, cancellationToken))
            throw new ConflictException(message);
    }
}