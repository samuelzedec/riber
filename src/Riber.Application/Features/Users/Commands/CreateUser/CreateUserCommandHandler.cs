using System.Net;
using Riber.Application.Abstractions.Commands;
using Riber.Application.Abstractions.Services;
using Riber.Application.Common;
using Riber.Application.Models.User;
using Riber.Domain.Repositories;

namespace Riber.Application.Features.Users.Commands.CreateUser;

internal sealed class CreateUserCommandHandler(
    IUnitOfWork unitOfWork,
    IUserCreationService userCreationService)
    : ICommandHandler<CreateUserCommand, CreateUserCommandResponse>
{
    public async ValueTask<Result<CreateUserCommandResponse>> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await userCreationService.CreateCompleteUserAsync(
                new CreateUserCompleteModel(
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

            if (!result.IsSuccess)
                return Result.Failure<CreateUserCommandResponse>(result.Error.Message, result.StatusCode);

            await unitOfWork.CommitTransactionAsync(cancellationToken);
            return Result.Success(
                new CreateUserCommandResponse(command.UserName, command.Email),
                HttpStatusCode.Created
            );
        }
        finally
        {
            if (unitOfWork.HasActiveTransaction())
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
        }
    }
}