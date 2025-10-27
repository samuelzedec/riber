using Bogus.Extensions.Brazil;
using FluentAssertions;
using Moq;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Features.Auths.Commands.Login;
using Riber.Application.Models.User;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Entities;
using Riber.Domain.Enums;
using Riber.Domain.Tests;

namespace Riber.Application.Tests.Features.Auths.Commands;

public sealed class LoginCommandHandlerTests : BaseTest
{
    #region Setup

    private readonly Mock<IAuthenticationService> _mockAuthService;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly UserDetailsModel _userDetailsTest;
    private readonly LoginCommand _command;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _mockAuthService = new Mock<IAuthenticationService>();
        _mockTokenService = new Mock<ITokenService>();

        var userDomain = User.Create(
            _faker.Name.FullName(),
            _faker.Person.Cpf(),
            BusinessPosition.Owner,
            Guid.CreateVersion7()
        );

        _userDetailsTest = CreateFaker<UserDetailsModel>()
            .CustomInstantiator(f => new UserDetailsModel(
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

        _command = CreateFaker<LoginCommand>()
            .CustomInstantiator(f => new LoginCommand(
                f.Internet.Email(),
                f.Internet.Password()
            ));

        _handler = new LoginCommandHandler(
            _mockAuthService.Object,
            _mockTokenService.Object
        );
    }

    #endregion

    #region Success Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should login successfully when valid credentials are provided")]
    public async Task Handle_WhenValidCredentials_ShouldReturnSuccessResult()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_userDetailsTest);

        _mockTokenService
            .Setup(x => x.GenerateToken(It.IsAny<UserDetailsModel>()))
            .Returns(_faker.Random.AlphaNumeric(32));

        _mockTokenService
            .Setup(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(_faker.Random.AlphaNumeric(32));

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.RefreshToken.Should().NotBeNullOrEmpty();
        result.Value.Token.Should().NotBeNullOrEmpty();
        result.Value.UserApplicationId.Should().Be(_userDetailsTest.Id);
        result.Value.UserDomainId.Should().Be(_userDetailsTest.UserDomainId);

        _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<UserDetailsModel>()), Times.Once);
        _mockTokenService.Verify(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
    }

    #endregion

    #region Exception Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return failure when user is not found")]
    public async Task Handle_WhenUserNotFound_ShouldReturnFailure()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((UserDetailsModel?)null);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be(AuthenticationErrors.InvalidCredentials);
        
        _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<UserDetailsModel>()), Times.Never);
        _mockTokenService.Verify(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return failure when password is incorrect")]
    public async Task Handle_WhenInvalidPassword_ShouldReturnFailure()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((UserDetailsModel?)null);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be(AuthenticationErrors.InvalidCredentials);
        
        _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<UserDetailsModel>()), Times.Never);
        _mockTokenService.Verify(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should propagate exception when unexpected exception occurs")]
    public async Task Handle_WhenUnexpectedExceptionOccurs_ShouldPropagateException()
    {
        // Arrange
        var exception = new InvalidOperationException("Database connection failed");
        _mockAuthService
            .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(exception);

        // Act
        var act = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Database connection failed");

        _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<UserDetailsModel>()), Times.Never);
        _mockTokenService.Verify(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    #endregion
}