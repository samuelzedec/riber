using System.Net;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Moq;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Dtos.User;
using Riber.Application.Features.Auths.Queries.GetRefreshToken;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Entities.User;
using Riber.Domain.Enums;
using Riber.Domain.Tests;

namespace Riber.Application.Tests.Features.Auths.Queries;

public sealed class GetRefreshTokenQueryHandlerTests : BaseTest
{
    #region Setup

    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IAuthenticationService> _mockAuthenticationService;
    private readonly Mock<IUserQueryService> _mockUserQueryService;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly UserDetailsDto _userDetailsTest;
    private readonly GetRefreshTokenQuery _query;
    private readonly GetRefreshTokenQueryHandler _handler;
    private readonly Guid _userId;

    public GetRefreshTokenQueryHandlerTests()
    {
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockAuthenticationService = new Mock<IAuthenticationService>();
        _mockUserQueryService = new Mock<IUserQueryService>();
        _mockTokenService = new Mock<ITokenService>();
        
        var userDomain = User.Create(
            _faker.Name.FullName(),
            _faker.Person.Cpf(),
            BusinessPosition.Owner,
            Guid.CreateVersion7()
        );

        _userId = userDomain.Id;
        _userDetailsTest = CreateFaker<UserDetailsDto>()
            .CustomInstantiator(f => new UserDetailsDto(
                Id: Guid.CreateVersion7(),
                UserName: f.Internet.UserName(),
                Email: f.Internet.Email(),
                EmailConfirmed: false,
                PhoneNumber: string.Empty,
                SecurityStamp: f.Random.AlphaNumeric(32),
                Roles: [.. f.Make(2, () => f.Name.JobTitle())],
                Claims: [],
                UserDomainId: userDomain.Id,
                UserDomain: userDomain
            ));

        _query = new GetRefreshTokenQuery();

        _handler = new GetRefreshTokenQueryHandler(
            _mockCurrentUserService.Object,
            _mockAuthenticationService.Object,
            _mockUserQueryService.Object,
            _mockTokenService.Object
        );
    }

    #endregion

    #region Success Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should generate refresh token successfully when valid user is provided")]
    public async Task Handle_WhenValidUser_ShouldReturnSuccessResult()
    {
        // Arrange
        _mockCurrentUserService
            .Setup(x => x.GetUserId())
            .Returns(_userId);

        _mockAuthenticationService
            .Setup(x => x.RefreshSecurityStampAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockUserQueryService
            .Setup(x => x.FindByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDetailsTest);

        _mockTokenService
            .Setup(x => x.GenerateToken(It.IsAny<UserDetailsDto>()))
            .Returns(_faker.Random.AlphaNumeric(32));

        _mockTokenService
            .Setup(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(_faker.Random.AlphaNumeric(32));

        // Act
        var result = await _handler.Handle(_query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Token.Should().NotBeNullOrEmpty();
        result.Value.RefreshToken.Should().NotBeNullOrEmpty();
        result.Value.UserApplicationId.Should().Be(_userDetailsTest.Id);
        result.Value.UserDomainId.Should().Be(_userDetailsTest.UserDomainId);

        _mockCurrentUserService.Verify(x => x.GetUserId(), Times.Once);
        _mockAuthenticationService.Verify(x => x.RefreshSecurityStampAsync(_userId.ToString()), Times.Once);
        _mockUserQueryService.Verify(x => x.FindByIdAsync(_userId), Times.Once);
        _mockTokenService.Verify(x => x.GenerateToken(_userDetailsTest), Times.Once);
        _mockTokenService.Verify(x => x.GenerateRefreshToken(_userDetailsTest.Id, _userDetailsTest.SecurityStamp), Times.Once);
    }

    #endregion

    #region Failure Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return failure when RefreshSecurityStampAsync fails")]
    public async Task Handle_WhenRefreshSecurityStampAsyncFails_ShouldReturnFailure()
    {
        // Arrange
        _mockCurrentUserService
            .Setup(x => x.GetUserId())
            .Returns(_userId);

        _mockAuthenticationService
            .Setup(x => x.RefreshSecurityStampAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(_query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be(AuthenticationErrors.InvalidCredentials);
        
        _mockCurrentUserService.Verify(x => x.GetUserId(), Times.Once);
        _mockAuthenticationService.Verify(x => x.RefreshSecurityStampAsync(_userId.ToString()), Times.Once);
        _mockUserQueryService.Verify(x => x.FindByIdAsync(It.IsAny<Guid>()), Times.Never);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<UserDetailsDto>()), Times.Never);
        _mockTokenService.Verify(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return failure when user not found")]
    public async Task Handle_WhenUserNotFound_ShouldReturnFailure()
    {
        // Arrange
        _mockCurrentUserService
            .Setup(x => x.GetUserId())
            .Returns(_userId);

        _mockAuthenticationService
            .Setup(x => x.RefreshSecurityStampAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockUserQueryService
            .Setup(x => x.FindByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserDetailsDto?)null);

        // Act
        var result = await _handler.Handle(_query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be(NotFoundErrors.User);
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        _mockCurrentUserService.Verify(x => x.GetUserId(), Times.Once);
        _mockAuthenticationService.Verify(x => x.RefreshSecurityStampAsync(_userId.ToString()), Times.Once);
        _mockUserQueryService.Verify(x => x.FindByIdAsync(_userId), Times.Once);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<UserDetailsDto>()), Times.Never);
        _mockTokenService.Verify(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    #endregion

    #region Exception Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should propagate exception when unexpected exception occurs in currentUserService")]
    public async Task Handle_WhenCurrentUserServiceThrowsUnexpectedException_ShouldPropagateException()
    {
        // Arrange
        var exception = new InvalidOperationException("Unable to retrieve user context");
        
        _mockCurrentUserService
            .Setup(x => x.GetUserId())
            .Throws(exception);

        // Act
        var act = async () => await _handler.Handle(_query, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Unable to retrieve user context");
        
        _mockCurrentUserService.Verify(x => x.GetUserId(), Times.Once);
        _mockAuthenticationService.Verify(x => x.RefreshSecurityStampAsync(It.IsAny<string>()), Times.Never);
        _mockUserQueryService.Verify(x => x.FindByIdAsync(It.IsAny<Guid>()), Times.Never);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<UserDetailsDto>()), Times.Never);
        _mockTokenService.Verify(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should propagate exception when unexpected exception occurs in FindByIdAsync")]
    public async Task Handle_WhenFindByIdAsyncThrowsUnexpectedException_ShouldPropagateException()
    {
        // Arrange
        var exception = new InvalidOperationException("Database connection failed during user retrieval");
        
        _mockCurrentUserService
            .Setup(x => x.GetUserId())
            .Returns(_userId);

        _mockAuthenticationService
            .Setup(x => x.RefreshSecurityStampAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockUserQueryService
            .Setup(x => x.FindByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(exception);

        // Act
        var act = async () => await _handler.Handle(_query, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Database connection failed during user retrieval");
        
        _mockCurrentUserService.Verify(x => x.GetUserId(), Times.Once);
        _mockAuthenticationService.Verify(x => x.RefreshSecurityStampAsync(_userId.ToString()), Times.Once);
        _mockUserQueryService.Verify(x => x.FindByIdAsync(_userId), Times.Once);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<UserDetailsDto>()), Times.Never);
        _mockTokenService.Verify(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    #endregion

    #region Parameter Validation Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should call services with correct parameters")]
    public async Task Handle_WhenCalled_ShouldCallServicesWithCorrectParameters()
    {
        // Arrange
        _mockCurrentUserService
            .Setup(x => x.GetUserId())
            .Returns(_userId);

        _mockAuthenticationService
            .Setup(x => x.RefreshSecurityStampAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockUserQueryService
            .Setup(x => x.FindByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_userDetailsTest);

        _mockTokenService
            .Setup(x => x.GenerateToken(It.IsAny<UserDetailsDto>()))
            .Returns(_faker.Random.AlphaNumeric(32));

        _mockTokenService
            .Setup(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(_faker.Random.AlphaNumeric(32));

        // Act
        await _handler.Handle(_query, CancellationToken.None);

        // Assert
        _mockAuthenticationService.Verify(
            x => x.RefreshSecurityStampAsync(_userId.ToString()), 
            Times.Once);
        
        _mockUserQueryService.Verify(
            x => x.FindByIdAsync(_userId), 
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

