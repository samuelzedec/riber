using FluentAssertions;
using Moq;
using Riber.Application.Abstractions.Services;
using Riber.Application.Features.Auths.Queries.GetPermissions;
using Riber.Domain.Tests;

namespace Riber.Application.Tests.Features.Auths.Queries;

public sealed class GetPermissionsQueryHandlerTests : BaseTest
{
    #region Setup

    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly GetPermissionsQuery _query;
    private readonly GetPermissionsQueryHandler _handler;

    public GetPermissionsQueryHandlerTests()
    {
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _query = new GetPermissionsQuery();

        _handler = new GetPermissionsQueryHandler(
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