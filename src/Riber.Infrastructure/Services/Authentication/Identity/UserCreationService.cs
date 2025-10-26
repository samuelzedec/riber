using System.Net;
using Riber.Application.Abstractions.Services;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Common;
using Riber.Application.Models.User;
using Riber.Domain.Abstractions;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Entities;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.User;
using Riber.Domain.ValueObjects.Email;
using Riber.Domain.ValueObjects.Phone;

namespace Riber.Infrastructure.Services.Authentication.Identity;

public sealed class UserCreationService(
    IUnitOfWork unitOfWork,
    IUserManagementService userManagementService,
    IUserQueryService userQueryService)
    : IUserCreationService
{
    public async Task<Result<EmptyResult>> CreateCompleteUserAsync(
        CreateUserCompleteModel model,
        CancellationToken cancellationToken = default)
    {
        var validateResult = await ValidateUserDoesNotExistAsync(
            model.Email,
            model.UserName,
            model.PhoneNumber,
            model.TaxId
        );

        if (validateResult is not null)
            return Result.Failure(validateResult, HttpStatusCode.Conflict);

        var domainUser = User.Create(
            fullName: model.FullName,
            taxId: model.TaxId,
            position: model.Position,
            companyId: model.CompanyId
        );

        var applicationUser = new CreateApplicationUserModel(
            UserName: model.UserName,
            Name: model.FullName.Split(' ')[0],
            Email: Email.Create(model.Email).Value,
            PhoneNumber: Phone.Create(model.PhoneNumber).Value,
            Password: model.Password,
            UserDomainId: domainUser.Id,
            Roles: model.Roles
        );

        await unitOfWork.Users.CreateAsync(domainUser, cancellationToken);
        return await userManagementService.CreateUserAsync(applicationUser);
    }

    /// <summary>
    /// Valida que não existe um usuário no sistema baseado no email, nome de usuário, número de telefone ou CPF/CNPJ.
    /// </summary>
    /// <param name="email">O endereço de email para verificar usuários existentes.</param>
    /// <param name="userName">O nome de usuário para verificar usuários existentes.</param>
    /// <param name="phoneNumber">O número de telefone para verificar usuários existentes.</param>  
    /// <param name="taxId">O CPF/CNPJ para verificar usuários existentes.</param>
    /// <returns>Mensagem de erro se encontrar duplicata, ou null se tudo estiver válido.</returns>
    private async Task<string?> ValidateUserDoesNotExistAsync(
        string email,
        string userName,
        string phoneNumber,
        string taxId)
    {
        if ((await userQueryService.FindByEmailAsync(email)).IsSuccess)
            return ConflictErrors.Email;

        if ((await userQueryService.FindByUserNameAsync(userName)).IsSuccess)
            return ConflictErrors.UserName;

        if ((await userQueryService.FindByPhoneAsync(phoneNumber)).IsSuccess)
            return ConflictErrors.Phone;

        if (await unitOfWork.Users.ExistsAsync(new UserTaxIdSpecification(IDocumentValidator.SanitizeStatic(taxId))))
            return ConflictErrors.TaxId;

        return null;
    }
}