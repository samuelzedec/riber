using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.DTOs;
using SnackFlow.Application.Exceptions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Entities;
using SnackFlow.Domain.Repositories;
using SnackFlow.Domain.ValueObjects.Email;
using SnackFlow.Domain.ValueObjects.Phone;

namespace SnackFlow.Infrastructure.Services;

public class UserCreationService(
    IUnitOfWork unitOfWork, 
    IAuthService authService) 
    : IUserCreationService
{
    public async Task CreateCompleteUserAsync(
        CreateUserCompleteDTO dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateUserDoesNotExistAsync(dto.Email, dto.UserName, dto.PhoneNumber, dto.TaxId);

        var domainUser = User.Create(
            fullName: dto.FullName,
            taxId: dto.TaxId,
            position: dto.Position,
            companyId: dto.CompanyId
        );

        var applicationUser = new CreateApplicationUserDTO(
            UserName: dto.UserName,
            Name: dto.FullName.Split(' ')[0],
            Email: Email.Create(dto.Email).Value,
            PhoneNumber: Phone.Create(dto.PhoneNumber).Value,
            Password: dto.Password,
            UserDomainId: domainUser.Id,
            Roles: dto.Roles
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
            throw new ConflictException(ErrorMessage.Conflict.EmailAlreadyExists);

        if (await authService.FindByUserNameAsync(userName) is not null)
            throw new ConflictException(ErrorMessage.Conflict.UserNameAlreadyExists);

        if (await authService.FindByPhoneAsync(phoneNumber) is not null)
            throw new ConflictException(ErrorMessage.Conflict.PhoneAlreadyExists);

        if (await unitOfWork.Users.ExistsAsync(x => x.TaxId.Value == taxId))
            throw new ConflictException(ErrorMessage.Conflict.TaxIdAlreadyExists);
    }
}