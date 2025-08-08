using System.Linq.Expressions;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.DTOs;
using SnackFlow.Application.Exceptions;
using SnackFlow.Application.Features.Users.Commands.CreateUser;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Entities;
using SnackFlow.Domain.Enums;
using SnackFlow.Domain.Repositories;
using SnackFlow.Domain.Tests;

namespace SnackFlow.Application.Tests.Features.Users.Commands;

public class CreateUserCommandHandlerUnitTests : BaseTest
{
    private readonly Mock<IUnitOfWork> _mockUnitOfOWork;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPermissionDataService> _mockPermissionDataService;
    private readonly Mock<ILogger<CreateUserCommandHandler>> _mockLogger;
    private readonly CreateUserCommandHandler _handler;
    private readonly CreateUserCommand _command;

    public CreateUserCommandHandlerUnitTests()
    {
        _mockUnitOfOWork = new Mock<IUnitOfWork>();
        _mockAuthService = new Mock<IAuthService>();
        _mockPermissionDataService = new Mock<IPermissionDataService>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<CreateUserCommandHandler>>();

        _handler = new CreateUserCommandHandler(
            _mockUnitOfOWork.Object,
            _mockAuthService.Object,
            _mockPermissionDataService.Object,
            _mockLogger.Object
        );

        _command = new CreateUserCommand(
            FullName: _faker.Person.FullName,
            TaxId: _faker.Person.Cpf(),
            Position: BusinessPosition.Employee,
            CompanyId: null,
            UserName: _faker.Person.UserName,
            Password: _faker.Random.String2(10),
            Email: _faker.Person.Email,
            PhoneNumber: _faker.Phone.PhoneNumber("(92) 9####-####")
        );
    }

    [Fact(DisplayName = "Creating user with valid data should return success response")]
    public async Task Handle_WhenValidData_ShouldReturnSuccessResponse()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDTO?)null);

        _mockAuthService
            .Setup(x => x.FindByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDTO?)null);

        _mockAuthService
            .Setup(x => x.FindByPhoneAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDTO?)null);

        _mockUnitOfOWork
            .Setup(x => x.Users)
            .Returns(_mockUserRepository.Object);

        _mockUserRepository
            .Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockUserRepository
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()));

        _mockUnitOfOWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        _mockAuthService
            .Setup(x => x.CreateAsync(It.IsAny<CreateApplicationUserDTO>(), It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Value.Email.Should().Be(_command.Email);
        result.Value.UserName.Should().Be(_command.UserName);

        _mockAuthService.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockAuthService.Verify(x => x.FindByUserNameAsync(It.IsAny<string>()), Times.Once);
        _mockAuthService.Verify(x => x.FindByPhoneAsync(It.IsAny<string>()), Times.Once);
        _mockUnitOfOWork.Verify(x => x.Users, Times.Exactly(2));
        _mockUserRepository.Verify(
            x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfOWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockAuthService.Verify(x => x.CreateAsync(It.IsAny<CreateApplicationUserDTO>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Creating user when email already exists should throw ConflictException")]
    public async Task Handle_WhenEmailAlreadyExists_ShouldThrowConflictException()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserDetailsDTO());

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<ConflictException>()
            .WithMessage(ErrorMessage.Conflict.EmailAlreadyExists);

        _mockAuthService.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockAuthService.Verify(x => x.FindByUserNameAsync(It.IsAny<string>()), Times.Never);
        _mockAuthService.Verify(x => x.FindByPhoneAsync(It.IsAny<string>()), Times.Never);
        _mockUnitOfOWork.Verify(x => x.Users, Times.Never);
        _mockUserRepository.Verify(
            x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfOWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockAuthService.Verify(x => x.CreateAsync(It.IsAny<CreateApplicationUserDTO>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact(DisplayName = "Creating user when username already exists should throw ConflictException")]
    public async Task Handle_WhenUserNameAlreadyExists_ShouldThrowConflictException()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDTO?)null);

        _mockAuthService
            .Setup(x => x.FindByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserDetailsDTO());

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<ConflictException>()
            .WithMessage(ErrorMessage.Conflict.UserNameAlreadyExists);

        _mockAuthService.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockAuthService.Verify(x => x.FindByUserNameAsync(It.IsAny<string>()), Times.Once);
        _mockAuthService.Verify(x => x.FindByPhoneAsync(It.IsAny<string>()), Times.Never);
        _mockUnitOfOWork.Verify(x => x.Users, Times.Never);
        _mockUserRepository.Verify(
            x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfOWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockAuthService.Verify(x => x.CreateAsync(It.IsAny<CreateApplicationUserDTO>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact(DisplayName = "Creating user when phone already exists should throw ConflictException")]
    public async Task Handle_WhenPhoneAlreadyExists_ShouldThrowConflictException()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDTO?)null);

        _mockAuthService
            .Setup(x => x.FindByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDTO?)null);

        _mockAuthService
            .Setup(x => x.FindByPhoneAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserDetailsDTO());

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<ConflictException>()
            .WithMessage(ErrorMessage.Conflict.PhoneAlreadyExists);

        _mockAuthService.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockAuthService.Verify(x => x.FindByUserNameAsync(It.IsAny<string>()), Times.Once);
        _mockAuthService.Verify(x => x.FindByPhoneAsync(It.IsAny<string>()), Times.Once);
        _mockUnitOfOWork.Verify(x => x.Users, Times.Never);
        _mockUserRepository.Verify(
            x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfOWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockAuthService.Verify(x => x.CreateAsync(It.IsAny<CreateApplicationUserDTO>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact(DisplayName = "Creating user when tax ID already exists should throw ConflictException")]
    public async Task Handle_WhenTaxIdAlreadyExists_ShouldThrowConflictException()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDTO?)null);

        _mockAuthService
            .Setup(x => x.FindByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDTO?)null);

        _mockAuthService
            .Setup(x => x.FindByPhoneAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDTO?)null);

        _mockUnitOfOWork
            .Setup(x => x.Users)
            .Returns(_mockUserRepository.Object);

        _mockUserRepository
            .Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<ConflictException>()
            .WithMessage(ErrorMessage.Conflict.TaxIdAlreadyExists);

        _mockAuthService.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockAuthService.Verify(x => x.FindByUserNameAsync(It.IsAny<string>()), Times.Once);
        _mockAuthService.Verify(x => x.FindByPhoneAsync(It.IsAny<string>()), Times.Once);
        _mockUnitOfOWork.Verify(x => x.Users, Times.Once);
        _mockUserRepository.Verify(
            x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfOWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockAuthService.Verify(x => x.CreateAsync(It.IsAny<CreateApplicationUserDTO>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact(DisplayName = "Creating user when domain creation fails should log error and rethrow")]
    public async Task Handle_WhenDomainCreationFails_ShouldLogErrorAndRethrow()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDTO?)null);

        _mockAuthService
            .Setup(x => x.FindByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDTO?)null);

        _mockAuthService
            .Setup(x => x.FindByPhoneAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDTO?)null);

        _mockUnitOfOWork
            .Setup(x => x.Users)
            .Returns(_mockUserRepository.Object);

        _mockUserRepository
            .Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockUserRepository
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Database error");

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "Creating user when save changes fails should log error and rethrow")]
    public async Task Handle_WhenSaveChangesFails_ShouldLogErrorAndRethrow()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDTO?)null);

        _mockAuthService
            .Setup(x => x.FindByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDTO?)null);

        _mockAuthService
            .Setup(x => x.FindByPhoneAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDTO?)null);

        _mockUnitOfOWork
            .Setup(x => x.Users)
            .Returns(_mockUserRepository.Object);

        _mockUserRepository
            .Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockUserRepository
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfOWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database save error"));

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Database save error");

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "Creating user when auth service fails should log error and rethrow")]
    public async Task Handle_WhenAuthServiceFails_ShouldLogErrorAndRethrow()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDTO?)null);

        _mockAuthService
            .Setup(x => x.FindByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDTO?)null);

        _mockAuthService
            .Setup(x => x.FindByPhoneAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDTO?)null);

        _mockUnitOfOWork
            .Setup(x => x.Users)
            .Returns(_mockUserRepository.Object);

        _mockUserRepository
            .Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockUserRepository
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()));

        _mockUnitOfOWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        _mockAuthService
            .Setup(x => x.CreateAsync(It.IsAny<CreateApplicationUserDTO>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Identity service error"));

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Identity service error");

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfOWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}