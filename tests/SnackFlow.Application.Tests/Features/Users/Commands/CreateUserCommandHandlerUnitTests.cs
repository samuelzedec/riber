using Bogus.Extensions.Brazil;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.DTOs;
using SnackFlow.Application.Exceptions;
using SnackFlow.Application.Features.Users.Commands.CreateUser;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Enums;
using SnackFlow.Domain.Repositories;
using SnackFlow.Domain.Tests;

namespace SnackFlow.Application.Tests.Features.Users.Commands;

public class CreateUserCommandHandlerUnitTests : BaseTest
{
    #region Setup

    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserCreationService> _mockUserCreationService;
    private readonly Mock<ILogger<CreateUserCommandHandler>> _mockLogger;
    private readonly CreateUserCommandHandler _handler;
    private readonly CreateUserCommand _command;

    public CreateUserCommandHandlerUnitTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserCreationService = new Mock<IUserCreationService>();
        _mockLogger = new Mock<ILogger<CreateUserCommandHandler>>();

        _handler = new CreateUserCommandHandler(
            _mockUnitOfWork.Object,
            _mockUserCreationService.Object,
            _mockLogger.Object
        );

        _command = new CreateUserCommand(
            FullName: _faker.Person.FullName,
            TaxId: _faker.Person.Cpf(),
            Position: _faker.PickRandom<BusinessPosition>(),
            CompanyId: null,
            UserName: _faker.Person.UserName,
            Password: _faker.Random.String2(10),
            Email: _faker.Person.Email,
            PhoneNumber: _faker.Phone.PhoneNumber("(92) 9####-####")
        );
    }

    #endregion

    #region Success Tests

    [Fact(DisplayName = "Creating user with valid data should return success response")]
    public async Task Handle_WhenValidData_ShouldReturnSuccessResponse()
    {
        // Arrange
        _mockUserCreationService
            .Setup(x => x.CreateCompleteUserAsync(
                It.IsAny<CreateUserCompleteDTO>(),
                It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Email.Should().Be(_command.Email);
        result.Value.UserName.Should().Be(_command.UserName);

        _mockUserCreationService.Verify(x => x.CreateCompleteUserAsync(
            It.Is<CreateUserCompleteDTO>(dto => 
                dto.FullName == _command.FullName &&
                dto.UserName == _command.UserName &&
                dto.Email == _command.Email &&
                dto.Password == _command.Password &&
                dto.PhoneNumber == _command.PhoneNumber &&
                dto.TaxId == _command.TaxId &&
                dto.Position == _command.Position &&
                dto.CompanyId == _command.CompanyId &&
                dto.Roles.Contains("Viewer")),
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Conflict Tests

    [Fact(DisplayName = "Creating user when user creation service throws ConflictException should rollback and rethrow")]
    public async Task Handle_WhenUserCreationServiceThrowsConflictException_ShouldRollbackAndRethrow()
    {
        // Arrange
        _mockUserCreationService
            .Setup(x => x.CreateCompleteUserAsync(
                It.IsAny<CreateUserCompleteDTO>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ConflictException(ErrorMessage.Conflict.EmailAlreadyExists));

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<ConflictException>()
            .WithMessage(ErrorMessage.Conflict.EmailAlreadyExists);

        _mockUserCreationService.Verify(x => x.CreateCompleteUserAsync(
            It.IsAny<CreateUserCompleteDTO>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Never);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    #endregion

    #region Exception Tests

    [Fact(DisplayName = "Creating user when user creation service fails should log error and rethrow")]
    public async Task Handle_WhenUserCreationServiceFails_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var expectedException = new InvalidOperationException("User creation service error");

        _mockUserCreationService
            .Setup(x => x.CreateCompleteUserAsync(
                It.IsAny<CreateUserCompleteDTO>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("User creation service error");

        _mockUserCreationService.Verify(x => x.CreateCompleteUserAsync(
            It.IsAny<CreateUserCompleteDTO>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Never);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "Creating user when commit transaction fails should log error and rethrow")]
    public async Task Handle_WhenCommitTransactionFails_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Commit transaction error");

        _mockUserCreationService
            .Setup(x => x.CreateCompleteUserAsync(
                It.IsAny<CreateUserCompleteDTO>(),
                It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Commit transaction error");

        _mockUserCreationService.Verify(x => x.CreateCompleteUserAsync(
            It.IsAny<CreateUserCompleteDTO>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region Cancellation Tests

    [Fact(DisplayName = "Should respect cancellation token during user creation")]
    public async Task Handle_WhenCancellationTokenDuringUserCreation_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockCancellationToken = new CancellationToken(true);

        _mockUserCreationService
            .Setup(x => x.CreateCompleteUserAsync(
                It.IsAny<CreateUserCompleteDTO>(),
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)))
            .ThrowsAsync(new OperationCanceledException());

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = async () => await _handler.Handle(_command, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockUserCreationService.Verify(x => x.CreateCompleteUserAsync(
            It.IsAny<CreateUserCompleteDTO>(), 
            It.Is<CancellationToken>(ct => ct.IsCancellationRequested)), Times.Once);

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Should respect cancellation token during commit transaction")]
    public async Task Handle_WhenCancellationTokenDuringCommitTransaction_ShouldRespectCancellationToken()
    {
        // Arrange
        _mockUserCreationService
            .Setup(x => x.CreateCompleteUserAsync(
                It.IsAny<CreateUserCompleteDTO>(),
                It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockUserCreationService.Verify(x => x.CreateCompleteUserAsync(
            It.IsAny<CreateUserCompleteDTO>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}