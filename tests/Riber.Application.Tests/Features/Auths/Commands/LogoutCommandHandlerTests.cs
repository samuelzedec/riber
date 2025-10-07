using FluentAssertions;
using Moq;
using Riber.Application.Abstractions.Services;
using Riber.Application.Features.Auths.Commands.Logout;
using Riber.Domain.Tests;

namespace Riber.Application.Tests.Features.Auths.Commands;

public sealed class LogoutCommandHandlerTests : BaseTest
{
    #region Setup

    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly LogoutCommand _command;
    private readonly LogoutCommandHandler _handler;
    private readonly Guid _userId;

    public LogoutCommandHandlerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        
        _userId = Guid.CreateVersion7();
        _command = new LogoutCommand();

        _handler = new LogoutCommandHandler(
            _mockAuthService.Object,
            _mockCurrentUserService.Object
        );
    }

    #endregion

    #region Success Tests

    [Fact(DisplayName = "Should logout successfully when valid user is provided")]
    public async Task Handle_WhenValidUser_ShouldReturnSuccessResult()
    {
        // Arrange
        _mockCurrentUserService
            .Setup(x => x.GetUserId())
            .Returns(_userId);

        _mockAuthService
            .Setup(x => x.RefreshUserSecurityAsync(It.IsAny<string>()));

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        _mockCurrentUserService.Verify(x => x.GetUserId(), Times.Once);
        _mockAuthService.Verify(x => x.RefreshUserSecurityAsync(_userId.ToString()), Times.Once);
    }

    #endregion

    #region Exception Tests
    
    [Fact(DisplayName = "Should log error and rethrow when unexpected exception occurs in authService")]
    public async Task Handle_WhenAuthServiceThrowsUnexpectedException_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Database connection failed");
        
        _mockCurrentUserService
            .Setup(x => x.GetUserId())
            .Returns(_userId);

        _mockAuthService
            .Setup(x => x.RefreshUserSecurityAsync(It.IsAny<string>()))
            .ThrowsAsync(exception);

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Database connection failed");
        
        _mockCurrentUserService.Verify(x => x.GetUserId(), Times.Once);
        _mockAuthService.Verify(x => x.RefreshUserSecurityAsync(_userId.ToString()), Times.Once);
    }

    [Fact(DisplayName = "Should log error and rethrow when unexpected exception occurs in currentUserService")]
    public async Task Handle_WhenCurrentUserServiceThrowsUnexpectedException_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Unable to retrieve user context");
        
        _mockCurrentUserService
            .Setup(x => x.GetUserId())
            .Throws(exception);

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Unable to retrieve user context");
        
        _mockCurrentUserService.Verify(x => x.GetUserId(), Times.Once);
        _mockAuthService.Verify(x => x.RefreshUserSecurityAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Should call authService with correct user ID")]
    public async Task Handle_WhenCalled_ShouldCallAuthServiceWithCorrectUserId()
    {
        // Arrange
        _mockCurrentUserService
            .Setup(x => x.GetUserId())
            .Returns(_userId);

        _mockAuthService
            .Setup(x => x.RefreshUserSecurityAsync(It.IsAny<string>()));

        // Act
        await _handler.Handle(_command, CancellationToken.None);

        // Assert
        _mockAuthService.Verify(
            x => x.RefreshUserSecurityAsync(_userId.ToString()), 
            Times.Once);
    }

    #endregion
}