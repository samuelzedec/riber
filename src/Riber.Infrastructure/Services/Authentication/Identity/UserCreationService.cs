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

        if (!validateResult.IsSuccess)
            return validateResult;

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
        return await userManagementService.CreateUserAsync(applicationUser)
            ? Result.Success()
            : Result.Failure(UnexpectedErrors.Response, HttpStatusCode.InternalServerError);
    }

    /// <summary>
    /// Verifica se já existe um usuário no sistema com base no e-mail, nome de usuário, número de telefone ou CPF/CNPJ.
    /// </summary>
    /// <param name="email">Endereço de e-mail a ser verificado.</param>
    /// <param name="userName">Nome de usuário a ser verificado.</param>
    /// <param name="phoneNumber">Número de telefone a ser verificado.</param>  
    /// <param name="taxId">CPF ou CNPJ a ser verificado.</param>
    /// <returns>
    /// Um <see cref="Result&lt;EmptyResult&gt;"/> contendo a mensagem de conflito caso algum dado já exista no sistema, ou vazio se não houver duplicatas.
    /// </returns>
    private async Task<Result<EmptyResult>> ValidateUserDoesNotExistAsync(
        string email,
        string userName,
        string phoneNumber,
        string taxId)
    {
        if ((await userQueryService.FindByEmailAsync(email)) is not null)
            return Result.Failure(ConflictErrors.Email, HttpStatusCode.Conflict);

        if ((await userQueryService.FindByUserNameAsync(userName)) is not null)
            return Result.Failure(ConflictErrors.UserName, HttpStatusCode.Conflict);

        if ((await userQueryService.FindByPhoneAsync(phoneNumber)) is not null)
            return Result.Failure(ConflictErrors.Phone, HttpStatusCode.Conflict);

        if (await unitOfWork.Users.ExistsAsync(new UserTaxIdSpecification(IDocumentValidator.SanitizeStatic(taxId))))
            return Result.Failure(ConflictErrors.TaxId, HttpStatusCode.Conflict);

        return Result.Success();
    }
}