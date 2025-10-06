using Riber.Application.Abstractions.Services;
using Riber.Application.Exceptions;
using Riber.Application.Models;
using Riber.Domain.Abstractions;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Entities;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.User;
using Riber.Domain.ValueObjects.Email;
using Riber.Domain.ValueObjects.Phone;

namespace Riber.Infrastructure.Services;

public sealed class UserCreationService(
    IUnitOfWork unitOfWork,
    IAuthService authService)
    : IUserCreationService
{
    public async Task CreateCompleteUserAsync(
        CreateUserCompleteModel model,
        CancellationToken cancellationToken = default)
    {
        await ValidateUserDoesNotExistAsync(model.Email, model.UserName, model.PhoneNumber, model.TaxId);

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
        await authService.CreateAsync(applicationUser, cancellationToken);
    }

    /// <summary>
    /// Valida que não existe um usuário no sistema baseado no email, nome de usuário, número de telefone ou CPF/CNPJ.
    /// </summary>
    /// <param name="email">O endereço de email para verificar usuários existentes.</param>
    /// <param name="userName">O nome de usuário para verificar usuários existentes.</param>
    /// <param name="phoneNumber">O número de telefone para verificar usuários existentes.</param>  
    /// <param name="taxId">O CPF/CNPJ para verificar usuários existentes.</param>
    /// <exception cref="ConflictException">
    /// Lançada se um usuário com o email, nome de usuário, número de telefone ou CPF/CNPJ especificado já existir no sistema.
    /// </exception>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    private async Task ValidateUserDoesNotExistAsync(string email, string userName, string phoneNumber, string taxId)
    {
        if (await authService.FindByEmailAsync(email) is not null)
            throw new ConflictException(ConflictErrors.Email);

        if (await authService.FindByUserNameAsync(userName) is not null)
            throw new ConflictException(ConflictErrors.UserName);

        if (await authService.FindByPhoneAsync(phoneNumber) is not null)
            throw new ConflictException(ConflictErrors.Phone);

        if (await unitOfWork.Users.ExistsAsync(new UserTaxIdSpecification(IDocumentValidator.SanitizeStatic(taxId))))
            throw new ConflictException(ConflictErrors.TaxId);
    }
}