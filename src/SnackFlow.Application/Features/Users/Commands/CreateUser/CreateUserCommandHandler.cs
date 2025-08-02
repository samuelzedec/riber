using System.Transactions;
using Microsoft.Extensions.Logging;
using SnackFlow.Application.Abstractions.Commands;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.Common;
using SnackFlow.Application.DTOs;
using SnackFlow.Application.Exceptions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Entities;
using SnackFlow.Domain.Repositories;
using SnackFlow.Domain.ValueObjects.Email;
using SnackFlow.Domain.ValueObjects.Phone;

namespace SnackFlow.Application.Features.Users.Commands.CreateUser;

internal sealed class CreateUserCommandHandler(
    IUnitOfWork unitOfWork,
    IAuthService  authService,
    ILogger<CreateUserCommandHandler> logger)
    : ICommandHandler<CreateUserCommand, CreateUserCommandResponse>
{
    public async ValueTask<Result<CreateUserCommandResponse>> Handle(CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.FromMinutes(1) },
            TransactionScopeAsyncFlowOption.Enabled
        );

        try
        {
            await VerifyIfUserExists(command);

            var domainUser = User.Create(
                fullName: command.FullName,
                taxId: command.TaxId,
                position: command.Position,
                companyId: command.CompanyId
            );

            var applicationUser = new CreateApplicationUserDTO
            {
                UserName = command.UserName,
                Email = Email.Create(command.Email).Value,
                Name = command.FullName.Split(' ')[0],
                Password = command.Password,
                PhoneNumber = Phone.Create(command.PhoneNumber).Value,
                UserDomainId = domainUser.Id
            };

            await unitOfWork.Users.CreateAsync(domainUser, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await authService.CreateAsync(applicationUser, cancellationToken);
            transaction.Complete();

            return new CreateUserCommandResponse(
                command.UserName,
                command.Email
            );
        }
        catch (ConflictException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessage.Exception.Unexpected(ex.GetType().Name, ex.Message));
            throw;
        }
    }

    private async Task VerifyIfUserExists(CreateUserCommand command)
    {
        Task<UserDetailsDTO?>[] userTasks =
        [
            authService.FindByEmailAsync(command.Email),
            authService.FindByUserNameAsync(command.UserName),
            authService.FindByPhoneAsync(command.PhoneNumber)
        ];

        var results = await Task.WhenAll(userTasks);
        
        if (results[0] is not null) 
            throw new ConflictException(ErrorMessage.Conflict.EmailAlreadyExists);
        
        if (results[1] is not null) 
            throw new ConflictException(ErrorMessage.Conflict.UserNameAlreadyExists);
        
        if (results[2] is not null) 
            throw new ConflictException(ErrorMessage.Conflict.PhoneAlreadyExists);
        
        if (await unitOfWork.Users.ExistsAsync(x => x.TaxId.Value == command.TaxId)) 
            throw new ConflictException(ErrorMessage.Conflict.TaxIdAlreadyExists);
    }
}