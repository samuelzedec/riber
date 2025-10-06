using Bogus.Extensions.Brazil;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Riber.Application.Abstractions.Services;
using Riber.Application.Exceptions;
using Riber.Application.Features.Auths.Queries.GetRefreshToken;
using Riber.Application.Models;
using Riber.Domain.Entities;
using Riber.Domain.Enums;
using Riber.Domain.Tests;

namespace Riber.Application.Tests.Features.Auths.Queries;

public sealed class GetRefreshTokenQueryHandlerTests : BaseTest
{
    #region Setup

    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<ILogger<GetRefreshTokenQueryHandler>> _mockLogger;
    private readonly UserDetailsModel _userDetailsTest;
    private readonly GetRefreshTokenQuery _query;
    private readonly GetRefreshTokenQueryHandler _handler;
    private readonly Guid _userId;

    public GetRefreshTokenQueryHandlerTests()
    {
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockAuthService = new Mock<IAuthService>();
        _mockTokenService = new Mock<ITokenService>();
        _mockLogger = new Mock<ILogger<GetRefreshTokenQueryHandler>>();
        
        var userDomain = User.Create(
            _faker.Name.FullName(),
            _faker.Person.Cpf(),
            BusinessPosition.Owner,
            Guid.CreateVersion7()
        );

        _userId = userDomain.Id;
        _userDetailsTest = CreateFaker<UserDetailsModel>()
            .CustomInstantiator(f => new UserDetailsModel(
                Id: Guid.CreateVersion7(),
                UserName: f.Internet.UserName(),
                Email: f.Internet.Email(),
                EmailConfirmed: false,
                PhoneNumber: string.Empty,
                SecurityStamp: f.Random.AlphaNumeric(32),
                Roles: f.Make(2, () => f.Name.JobTitle()).ToList(),
                Claims: [],
                UserDomainId: userDomain.Id,
                UserDomain: userDomain
            ));

        _query = new GetRefreshTokenQuery();

        _handler = new GetRefreshTokenQueryHandler(
            _mockCurrentUserService.Object,
            _mockAuthService.Object,
            _mockTokenService.Object,
            _mockLogger.Object
        );
    }

    #endregion

    #region Success Tests

    [Fact(DisplayName = "Should generate refresh token successfully when valid user is provided")]
    public async Task Handle_WhenValidUser_ShouldReturnSuccessResult()
    {
        // Arrange
        _mockCurrentUserService
            .Setup(x => x.GetUserId())
            .Returns(_userId);

        _mockAuthService
            .Setup(x => x.RefreshUserSecurityAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask); // RefreshUserSecurityAsync agora é void

        _mockAuthService
            .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(_userDetailsTest); // Agora usa FindByIdAsync separadamente

        _mockTokenService
            .Setup(x => x.GenerateToken(It.IsAny<UserDetailsModel>()))
            .Returns(_faker.Random.AlphaNumeric(32));

        _mockTokenService
            .Setup(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(_faker.Random.AlphaNumeric(32));

        // Act
        var result = await _handler.Handle(_query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Token.Should().NotBeNullOrEmpty();
        result.Value.RefreshToken.Should().NotBeNullOrEmpty();
        result.Value.UserApplicationId.Should().Be(_userDetailsTest.Id);
        result.Value.UserDomainId.Should().Be(_userDetailsTest.UserDomainId);

        _mockCurrentUserService.Verify(x => x.GetUserId(), Times.Once);
        _mockAuthService.Verify(x => x.RefreshUserSecurityAsync(_userId.ToString()), Times.Once);
        _mockAuthService.Verify(x => x.FindByIdAsync(_userId.ToString()), Times.Once); // Nova verificação
        _mockTokenService.Verify(x => x.GenerateToken(_userDetailsTest), Times.Once);
        _mockTokenService.Verify(x => x.GenerateRefreshToken(_userDetailsTest.Id, _userDetailsTest.SecurityStamp), Times.Once);
        
        _mockLogger.Verify(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never
        );
    }

    #endregion

    #region Exception Tests

    [Fact(DisplayName = "Should log error and rethrow when unexpected exception occurs in currentUserService")]
    public async Task Handle_WhenCurrentUserServiceThrowsUnexpectedException_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Unable to retrieve user context");
        
        _mockCurrentUserService
            .Setup(x => x.GetUserId())
            .Throws(exception);

        // Act
        var result = async () => await _handler.Handle(_query, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Unable to retrieve user context");
        
        _mockCurrentUserService.Verify(x => x.GetUserId(), Times.Once);
        _mockAuthService.Verify(x => x.RefreshUserSecurityAsync(It.IsAny<string>()), Times.Never);
        _mockAuthService.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Never);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<UserDetailsModel>()), Times.Never);
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

    [Fact(DisplayName = "Should log error and rethrow when unexpected exception occurs in RefreshUserSecurityAsync")]
    public async Task Handle_WhenRefreshUserSecurityAsyncThrowsUnexpectedException_ShouldLogErrorAndRethrow()
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
        var result = async () => await _handler.Handle(_query, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Database connection failed");
        
        _mockCurrentUserService.Verify(x => x.GetUserId(), Times.Once);
        _mockAuthService.Verify(x => x.RefreshUserSecurityAsync(_userId.ToString()), Times.Once);
        _mockAuthService.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Never);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<UserDetailsModel>()), Times.Never);
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

    [Fact(DisplayName = "Should log error and rethrow when user not found in FindByIdAsync")]
    public async Task Handle_WhenUserNotFound_ShouldLogErrorAndRethrow()
    {
        // Arrange
        _mockCurrentUserService
            .Setup(x => x.GetUserId())
            .Returns(_userId);

        _mockAuthService
            .Setup(x => x.RefreshUserSecurityAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _mockAuthService
            .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsModel?)null);

        // Act
        var result = async () => await _handler.Handle(_query, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>();
        
        _mockCurrentUserService.Verify(x => x.GetUserId(), Times.Once);
        _mockAuthService.Verify(x => x.RefreshUserSecurityAsync(_userId.ToString()), Times.Once);
        _mockAuthService.Verify(x => x.FindByIdAsync(_userId.ToString()), Times.Once);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<UserDetailsModel>()), Times.Never);
        _mockTokenService.Verify(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Should log error and rethrow when unexpected exception occurs in FindByIdAsync")]
    public async Task Handle_WhenFindByIdAsyncThrowsUnexpectedException_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Database connection failed during user retrieval");
        
        _mockCurrentUserService
            .Setup(x => x.GetUserId())
            .Returns(_userId);

        _mockAuthService
            .Setup(x => x.RefreshUserSecurityAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _mockAuthService
            .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ThrowsAsync(exception);

        // Act
        var result = async () => await _handler.Handle(_query, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Database connection failed during user retrieval");
        
        _mockCurrentUserService.Verify(x => x.GetUserId(), Times.Once);
        _mockAuthService.Verify(x => x.RefreshUserSecurityAsync(_userId.ToString()), Times.Once);
        _mockAuthService.Verify(x => x.FindByIdAsync(_userId.ToString()), Times.Once);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<UserDetailsModel>()), Times.Never);
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

    #region Parameter Validation Tests

    [Fact(DisplayName = "Should call services with correct parameters")]
    public async Task Handle_WhenCalled_ShouldCallServicesWithCorrectParameters()
    {
        // Arrange
        _mockCurrentUserService
            .Setup(x => x.GetUserId())
            .Returns(_userId);

        _mockAuthService
            .Setup(x => x.RefreshUserSecurityAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _mockAuthService
            .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(_userDetailsTest);

        _mockTokenService
            .Setup(x => x.GenerateToken(It.IsAny<UserDetailsModel>()))
            .Returns(_faker.Random.AlphaNumeric(32));

        _mockTokenService
            .Setup(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(_faker.Random.AlphaNumeric(32));

        // Act
        await _handler.Handle(_query, CancellationToken.None);

        // Assert
        _mockAuthService.Verify(
            x => x.RefreshUserSecurityAsync(_userId.ToString()), 
            Times.Once);
        
        _mockAuthService.Verify(
            x => x.FindByIdAsync(_userId.ToString()), 
            Times.Once);
        
        _mockTokenService.Verify(
            x => x.GenerateToken(_userDetailsTest), 
            Times.Once);
        
        _mockTokenService.Verify(
            x => x.GenerateRefreshToken(_userDetailsTest.Id, _userDetailsTest.SecurityStamp), 
            Times.Once);
    }

    #endregion
}