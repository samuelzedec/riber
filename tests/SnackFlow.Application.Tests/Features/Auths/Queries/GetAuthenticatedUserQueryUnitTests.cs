using FluentAssertions;
using Moq;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.Features.Auths.Queries.GetAuthenticatedUser;
using SnackFlow.Domain.Tests;

namespace SnackFlow.Application.Tests.Features.Auths.Queries;

public sealed class GetAuthenticatedUserQueryUnitTests : BaseTest
{
    #region Setup

    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly GetAuthenticatedUserQuery _query;
    private readonly GetAuthenticatedUserQueryHandler _handler;

    public GetAuthenticatedUserQueryUnitTests()
    {
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _query = new GetAuthenticatedUserQuery();

        _handler = new GetAuthenticatedUserQueryHandler(
            _mockCurrentUserService.Object
        );
    }

    #endregion

    #region Success Tests

    [Fact(DisplayName = "Should return authenticated user with permissions successfully")]
    public async Task Handle_WhenCalled_ShouldReturnSuccessResult()
    {
        // Arrange
        var expectedPermissions = _faker.Make(3, () => _faker.Random.String2(10)).ToArray();
        
        _mockCurrentUserService
            .Setup(x => x.GetPermissions())
            .Returns(expectedPermissions);

        // Act
        var result = await _handler.Handle(_query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Permissions.Should().BeEquivalentTo(expectedPermissions);
        
        _mockCurrentUserService.Verify(x => x.GetPermissions(), Times.Once);
    }

    [Fact(DisplayName = "Should return empty permissions when user has no permissions")]
    public async Task Handle_WhenUserHasNoPermissions_ShouldReturnEmptyPermissions()
    {
        // Arrange
        var emptyPermissions = Array.Empty<string>();
        
        _mockCurrentUserService
            .Setup(x => x.GetPermissions())
            .Returns(emptyPermissions);

        // Act
        var result = await _handler.Handle(_query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Permissions.Should().BeEmpty();
        
        _mockCurrentUserService.Verify(x => x.GetPermissions(), Times.Once);
    }

    [Fact(DisplayName = "Should call currentUserService exactly once")]
    public async Task Handle_WhenCalled_ShouldCallCurrentUserServiceOnce()
    {
        // Arrange
        var permissions = _faker.Make(2, () => _faker.Random.String2(8)).ToArray();
        
        _mockCurrentUserService
            .Setup(x => x.GetPermissions())
            .Returns(permissions);

        // Act
        await _handler.Handle(_query, CancellationToken.None);

        // Assert
        _mockCurrentUserService.Verify(x => x.GetPermissions(), Times.Once);
        _mockCurrentUserService.VerifyNoOtherCalls();
    }
    
    #endregion
}