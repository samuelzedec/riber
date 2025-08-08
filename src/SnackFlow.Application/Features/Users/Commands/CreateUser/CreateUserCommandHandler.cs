using System.Transactions;
using Microsoft.Extensions.Logging;
using SnackFlow.Application.Abstractions.Commands;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.Common;
using SnackFlow.Application.Exceptions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Enums;
using SnackFlow.Domain.Repositories;

namespace SnackFlow.Application.Features.Users.Commands.CreateUser;

internal sealed class CreateUserCommandHandler(
    IUnitOfWork unitOfWork,
    IAuthService  authService,
    IPermissionDataService permissionDataService,
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
            
            var applicationUser = CreateUserCommand.ToApplicationUserDto(command, await GetPermissions(command));
            var applicationUserId = await authService.CreateAsync(applicationUser, cancellationToken);
            var domainUser = CreateUserCommand.ToDomainUser(command, applicationUserId);
            
            await unitOfWork.Users.CreateAsync(domainUser, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            transaction.Complete();

            return new CreateUserCommandResponse(
                command.UserName,
                command.Email,
                domainUser.PublicToken
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

    private async Task<IEnumerable<string>> GetPermissions(CreateUserCommand command)
    {
        if (command.Position is not BusinessPosition.Owner)
            return [];

        var permissions = await permissionDataService.GetAllWithDescriptionsAsync();
        return permissions.Select(x => x.Name);

    }

    private async Task VerifyIfUserExists(CreateUserCommand command)
    {
        if (await authService.FindByEmailAsync(command.Email) is not null) 
            throw new ConflictException(ErrorMessage.Conflict.EmailAlreadyExists);
        
        if (await authService.FindByUserNameAsync(command.UserName) is not null) 
            throw new ConflictException(ErrorMessage.Conflict.UserNameAlreadyExists);
        
        if (await authService.FindByPhoneAsync(command.PhoneNumber) is not null) 
            throw new ConflictException(ErrorMessage.Conflict.PhoneAlreadyExists);
        
        if (await unitOfWork.Users.ExistsAsync(x => x.TaxId.Value == command.TaxId)) 
            throw new ConflictException(ErrorMessage.Conflict.TaxIdAlreadyExists);
    }
}