using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Commands;
using Riber.Application.Abstractions.Services;
using Riber.Application.Common;
using Riber.Application.DTOs;
using Riber.Application.Exceptions;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Repositories;

namespace Riber.Application.Features.Users.Commands.CreateUser;

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
            logger.LogError(UnexpectedErrors.ForLogging(nameof(CreateUserCommandHandler), ex));
            throw;
        }
    }
}