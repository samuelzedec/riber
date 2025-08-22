using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.DTOs;
using SnackFlow.Application.Exceptions;
using SnackFlow.Application.Features.Auths.Commands.Login;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Tests;

namespace SnackFlow.Application.Tests.Features.Auths.Commands;

public sealed class LoginCommandUnitTests : BaseTest
{
    #region Setup

    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<ILogger<LoginCommandHandler>> _mockLogger;
    private readonly UserDetailsDTO _userDetailsTest;
    private readonly LoginCommand _command;
    private readonly LoginCommandHandler _handler;

    public LoginCommandUnitTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockTokenService = new Mock<ITokenService>();
        _mockLogger = new Mock<ILogger<LoginCommandHandler>>();

        _userDetailsTest = CreateFaker<UserDetailsDTO>()
            .CustomInstantiator(f => new UserDetailsDTO(
                Id: Guid.CreateVersion7(),
                UserName: f.Internet.UserName(),
                Email: f.Internet.Email(),
                EmailConfirmed: false,
                PhoneNumber: string.Empty,
                SecurityStamp: f.Random.AlphaNumeric(32),
                Roles: f.Make(2, () => f.Name.JobTitle()).ToList(),
                Claims: [],
                UserDomainId: Guid.CreateVersion7()
            ));

        _command = CreateFaker<LoginCommand>()
            .CustomInstantiator(f => new LoginCommand(
                f.Internet.Email(),
                f.Internet.Password()
            ));

        _handler = new LoginCommandHandler(
            _mockAuthService.Object,
            _mockTokenService.Object,
            _mockLogger.Object
        );
    }

    #endregion

    #region Success Tests

    [Fact(DisplayName = "Should login successfully when valid credentials are provided")]
    public async Task Handle_WhenValidCredentials_ShouldReturnSuccessResult()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_userDetailsTest);

        _mockTokenService
            .Setup(x => x.GenerateToken(It.IsAny<UserDetailsDTO>()))
            .Returns(_faker.Random.AlphaNumeric(32));

        _mockTokenService
            .Setup(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(_faker.Random.AlphaNumeric(32));

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.RefreshToken.Should().NotBeNullOrEmpty();
        result.Value.Token.Should().NotBeNullOrEmpty();
        result.Value.UserApplicationId.Should().Be(_userDetailsTest.Id);
        result.Value.UserDomainId.Should().Be(_userDetailsTest.UserDomainId);

        _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<UserDetailsDTO>()), Times.Once);
        _mockTokenService.Verify(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
    }

    #endregion

    #region Exception Tests

    [Fact(DisplayName = "Should throw NotFoundException when user is not found")]
    public async Task Handle_WhenUserNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new NotFoundException(ErrorMessage.NotFound.User));

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>().WithMessage(ErrorMessage.NotFound.User);
        _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<UserDetailsDTO>()), Times.Never);
        _mockTokenService.Verify(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(ErrorMessage.NotFound.User)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "Should throw UnauthorizedException when password is incorrect")]
    public async Task Handle_WhenInvalidPassword_ShouldThrowUnauthorizedException()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(null as UserDetailsDTO);

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<UnauthorizedException>().WithMessage(ErrorMessage.Invalid.Password);
        _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<UserDetailsDTO>()), Times.Never);
        _mockTokenService.Verify(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        _mockLogger.Verify(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never
        );
    }
    
    [Fact(DisplayName = "Should log error and rethrow when unexpected exception occurs")]
    public async Task Handle_WhenUnexpectedExceptionOccurs_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Database connection failed");
        _mockAuthService
            .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(exception);

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Database connection failed");
    
        _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<UserDetailsDTO>()), Times.Never);
        _mockTokenService.Verify(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("InvalidOperationException")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion
}