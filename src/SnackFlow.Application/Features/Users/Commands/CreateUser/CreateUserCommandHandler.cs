using Microsoft.Extensions.Logging;
using SnackFlow.Application.Abstractions.Commands;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.Common;
using SnackFlow.Application.DTOs;
using SnackFlow.Application.Exceptions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Repositories;

namespace SnackFlow.Application.Features.Users.Commands.CreateUser;

internal sealed class CreateUserCommandHandler(
    IUnitOfWork unitOfWork,
    IUserCreationService userCreationService,
    ILogger<CreateUserCommandHandler> logger)
    : ICommandHandler<CreateUserCommand, CreateUserCommandResponse>
{
    public async ValueTask<Result<CreateUserCommandResponse>> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await userCreationService.CreateCompleteUserAsync(
                new CreateUserCompleteDTO(
                    FullName: command.FullName,
                    UserName: command.UserName,
                    Email: command.Email,
                    Password: command.Password,
                    PhoneNumber: command.PhoneNumber,
                    TaxId: command.TaxId,
                    Position: command.Position,
                    Roles: ["Viewer"],
                    CompanyId: command.CompanyId
                ),
                cancellationToken
            );

            await unitOfWork.CommitTransactionAsync(cancellationToken);
            return new CreateUserCommandResponse(
                command.UserName,
                command.Email
            );
        }
        catch (ConflictException)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            logger.LogError(ex, ErrorMessage.Exception.Unexpected(ex.GetType().Name, ex.Message));
            throw;
        }
    }
}