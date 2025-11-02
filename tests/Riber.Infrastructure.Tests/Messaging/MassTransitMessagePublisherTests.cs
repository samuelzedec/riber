using FluentAssertions;
using MassTransit;
using Moq;
using Riber.Infrastructure.Messaging;

namespace Riber.Infrastructure.Tests.Messaging;

public sealed class MassTransitMessagePublisherTests
{
    private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
    private readonly MassTransitMessagePublisher _publisher;

    public MassTransitMessagePublisherTests()
    {
        _mockPublishEndpoint = new Mock<IPublishEndpoint>();
        _publisher = new MassTransitMessagePublisher(_mockPublishEndpoint.Object);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "PublishAsync should publish message successfully")]
    public async Task PublishAsync_WithMessage_ShouldPublishSuccessfully()
    {
        // Arrange
        var testMessage = new { Content = "Test Content" };
        var cancellationToken = CancellationToken.None;

        _mockPublishEndpoint
            .Setup(x => x.Publish(
                testMessage,
                cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _publisher.PublishAsync(testMessage, cancellationToken);

        // Assert
        _mockPublishEndpoint.Verify(
            x => x.Publish(testMessage, cancellationToken),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "PublishAsync should handle cancellation token correctly")]
    public async Task PublishAsync_WithCancellationToken_ShouldPassTokenToPublishEndpoint()
    {
        // Arrange
        var testMessage = new { Content = "Test Content" };
        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        _mockPublishEndpoint
            .Setup(x => x.Publish(
                testMessage,
                cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _publisher.PublishAsync(testMessage, cancellationToken);

        // Assert
        _mockPublishEndpoint.Verify(
            x => x.Publish(testMessage, cancellationToken),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "PublishAsync should use default cancellation token when none provided")]
    public async Task PublishAsync_WithDefaultCancellationToken_ShouldPublishSuccessfully()
    {
        // Arrange
        var testMessage = new { Content = "Test Content" };

        _mockPublishEndpoint
            .Setup(x => x.Publish(
                testMessage,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _publisher.PublishAsync(testMessage);

        // Assert
        _mockPublishEndpoint.Verify(
            x => x.Publish(testMessage, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "PublishAsync should throw when publish endpoint throws")]
    public async Task PublishAsync_WhenPublishEndpointThrows_ShouldPropagateException()
    {
        // Arrange
        var testMessage = new { Content = "Test Content" };
        var cancellationToken = CancellationToken.None;
        var expectedException = new InvalidOperationException("Publish failed");

        _mockPublishEndpoint
            .Setup(x => x.Publish(
                testMessage,
                cancellationToken))
            .ThrowsAsync(expectedException);

        // Act
        var act = async () => await _publisher.PublishAsync(testMessage, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Publish failed");
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "PublishAsync should handle operation cancelled exception")]
    public async Task PublishAsync_WhenCancellationRequested_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var testMessage = new { Content = "Test Content" };
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var cancellationToken = cts.Token;

        _mockPublishEndpoint
            .Setup(x => x.Publish(
                testMessage,
                cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var act = async () => await _publisher.PublishAsync(testMessage, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "PublishAsync with different message types should work correctly")]
    public async Task PublishAsync_WithDifferentMessageTypes_ShouldPublishCorrectly()
    {
        // Arrange
        var message1 = new { Content = "First Message" };
        var message2 = new { Value = 42 };
        var cancellationToken = CancellationToken.None;

        _mockPublishEndpoint
            .Setup(x => x.Publish(message1, cancellationToken))
            .Returns(Task.CompletedTask);

        _mockPublishEndpoint
            .Setup(x => x.Publish(message2, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _publisher.PublishAsync(message1, cancellationToken);
        await _publisher.PublishAsync(message2, cancellationToken);

        // Assert
        _mockPublishEndpoint.Verify(
            x => x.Publish(message1, cancellationToken),
            Times.Once
        );
        _mockPublishEndpoint.Verify(
            x => x.Publish(message2, cancellationToken),
            Times.Once
        );
    }
}