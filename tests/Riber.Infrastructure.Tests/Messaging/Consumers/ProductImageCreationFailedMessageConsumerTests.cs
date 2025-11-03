using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Riber.Application.Abstractions.Services;
using Riber.Application.Messages;
using Riber.Infrastructure.Messaging.Consumers;

namespace Riber.Infrastructure.Tests.Messaging.Consumers;

public sealed class ProductImageCreationFailedMessageConsumerTests
{
    private readonly Mock<IImageStorageService> _mockImageStorageService;
    private readonly Mock<ILogger<ProductImageCreationFailedMessageConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<ProductImageCreationFailedMessage>> _mockConsumeContext;
    private readonly ProductImageCreationFailedMessageConsumer _consumer;

    public ProductImageCreationFailedMessageConsumerTests()
    {
        _mockImageStorageService = new Mock<IImageStorageService>();
        _mockLogger = new Mock<ILogger<ProductImageCreationFailedMessageConsumer>>();
        _mockConsumeContext = new Mock<ConsumeContext<ProductImageCreationFailedMessage>>();

        _consumer = new ProductImageCreationFailedMessageConsumer(
            _mockImageStorageService.Object,
            _mockLogger.Object
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should delete image successfully")]
    public async Task Consume_WithValidImageKey_ShouldDeleteImageSuccessfully()
    {
        // Arrange
        const string imageKey = "products/test-image.jpg";
        var message = new ProductImageCreationFailedMessage(imageKey);

        _mockConsumeContext.Setup(x => x.Message).Returns(message);

        _mockImageStorageService
            .Setup(x => x.DeleteAllAsync(It.Is<List<string>>(list => list.Contains(imageKey))))
            .ReturnsAsync([imageKey]);

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        _mockImageStorageService.Verify(
            x => x.DeleteAllAsync(It.Is<List<string>>(list => list.Contains(imageKey))),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should log information when deleting image")]
    public async Task Consume_WhenDeletingImage_ShouldLogInformation()
    {
        // Arrange
        const string imageKey = "products/test-image.jpg";
        var message = new ProductImageCreationFailedMessage(imageKey);

        _mockConsumeContext.Setup(x => x.Message).Returns(message);

        _mockImageStorageService
            .Setup(x => x.DeleteAllAsync(It.Is<List<string>>(list => list.Contains(imageKey))))
            .ReturnsAsync([imageKey]);

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Starting deletion of image {imageKey}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Successfully deleted image {imageKey}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should log warning and return when image key is null")]
    public async Task Consume_WithNullImageKey_ShouldLogWarningAndReturn()
    {
        // Arrange
        var message = new ProductImageCreationFailedMessage(null!);

        _mockConsumeContext.Setup(x => x.Message).Returns(message);

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Image key is empty")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );

        _mockImageStorageService.Verify(
            x => x.DeleteAllAsync(It.IsAny<List<string>>()),
            Times.Never
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should log warning and return when image key is empty")]
    public async Task Consume_WithEmptyImageKey_ShouldLogWarningAndReturn()
    {
        // Arrange
        var message = new ProductImageCreationFailedMessage(string.Empty);

        _mockConsumeContext.Setup(x => x.Message).Returns(message);

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Image key is empty")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );

        _mockImageStorageService.Verify(
            x => x.DeleteAllAsync(It.IsAny<List<string>>()),
            Times.Never
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should log warning and return when image key is whitespace")]
    public async Task Consume_WithWhitespaceImageKey_ShouldLogWarningAndReturn()
    {
        // Arrange
        var message = new ProductImageCreationFailedMessage("   ");

        _mockConsumeContext.Setup(x => x.Message).Returns(message);

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Image key is empty")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );

        _mockImageStorageService.Verify(
            x => x.DeleteAllAsync(It.IsAny<List<string>>()),
            Times.Never
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should handle storage service exception gracefully")]
    public async Task Consume_WhenStorageServiceThrows_ShouldLogErrorAndNotThrow()
    {
        // Arrange
        const string imageKey = "products/test-image.jpg";
        var message = new ProductImageCreationFailedMessage(imageKey);
        var expectedException = new InvalidOperationException("Storage service unavailable");

        _mockConsumeContext.Setup(x => x.Message).Returns(message);

        _mockImageStorageService
            .Setup(x => x.DeleteAllAsync(It.Is<List<string>>(list => list.Contains(imageKey))))
            .ThrowsAsync(expectedException);

        // Act
        var act = async () => await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        await act.Should().NotThrowAsync();

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Failed to delete image {imageKey}")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should handle IOException gracefully")]
    public async Task Consume_WhenIOExceptionOccurs_ShouldLogErrorAndNotThrow()
    {
        // Arrange
        const string imageKey = "products/test-image.jpg";
        var message = new ProductImageCreationFailedMessage(imageKey);
        var expectedException = new IOException("Disk error");

        _mockConsumeContext.Setup(x => x.Message).Returns(message);

        _mockImageStorageService
            .Setup(x => x.DeleteAllAsync(It.Is<List<string>>(list => list.Contains(imageKey))))
            .ThrowsAsync(expectedException);

        // Act
        var act = async () => await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        await act.Should().NotThrowAsync();

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to delete image")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should delete image with special characters in key")]
    public async Task Consume_WithSpecialCharactersInKey_ShouldDeleteSuccessfully()
    {
        // Arrange
        const string imageKey = "products/test-image_123-456.jpg";
        var message = new ProductImageCreationFailedMessage(imageKey);

        _mockConsumeContext.Setup(x => x.Message).Returns(message);

        _mockImageStorageService
            .Setup(x => x.DeleteAllAsync(It.Is<List<string>>(list => list.Contains(imageKey))))
            .ReturnsAsync([imageKey]);

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        _mockImageStorageService.Verify(
            x => x.DeleteAllAsync(It.Is<List<string>>(list => list.Contains(imageKey))),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should call DeleteAllAsync exactly once")]
    public async Task Consume_ShouldCallDeleteAllAsyncOnce()
    {
        // Arrange
        const string imageKey = "products/test-image.jpg";
        var message = new ProductImageCreationFailedMessage(imageKey);

        _mockConsumeContext.Setup(x => x.Message).Returns(message);

        _mockImageStorageService
            .Setup(x => x.DeleteAllAsync(It.Is<List<string>>(list => list.Contains(imageKey))))
            .ReturnsAsync([imageKey]);

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        _mockImageStorageService.Verify(
            x => x.DeleteAllAsync(It.Is<List<string>>(list => list.Contains(imageKey))),
            Times.Once
        );

        _mockImageStorageService.VerifyNoOtherCalls();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should log both start and success messages in correct order")]
    public async Task Consume_ShouldLogStartAndSuccessInOrder()
    {
        // Arrange
        const string imageKey = "products/test-image.jpg";
        var message = new ProductImageCreationFailedMessage(imageKey);
        var logMessages = new List<string>();

        _mockConsumeContext.Setup(x => x.Message).Returns(message);

        _mockLogger
            .Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback<LogLevel, EventId, object, Exception, Delegate>((_, _, state, _, _) => logMessages.Add(state.ToString()!));

        _mockImageStorageService
            .Setup(x => x.DeleteAllAsync(It.Is<List<string>>(list => list.Contains(imageKey))))
            .ReturnsAsync([imageKey]);

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        logMessages.Should().HaveCount(2);
        logMessages[0].Should().Contain("Starting deletion");
        logMessages[1].Should().Contain("Successfully deleted");
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should handle multiple different image keys")]
    public async Task Consume_WithMultipleDifferentImageKeys_ShouldHandleEach()
    {
        // Arrange
        var imageKeys = new[]
        {
            "products/image1.jpg",
            "products/image2.png",
            "products/subfolder/image3.webp"
        };

        foreach (var imageKey in imageKeys)
        {
            var message = new ProductImageCreationFailedMessage(imageKey);
            var mockContext = new Mock<ConsumeContext<ProductImageCreationFailedMessage>>();
            mockContext.Setup(x => x.Message).Returns(message);

            _mockImageStorageService
                .Setup(x => x.DeleteAllAsync(It.Is<List<string>>(list => list.Contains(imageKey))))
                .ReturnsAsync([imageKey]);

            // Act
            await _consumer.Consume(mockContext.Object);
        }

        // Assert
        foreach (var imageKey in imageKeys)
        {
            _mockImageStorageService.Verify(
                x => x.DeleteAllAsync(It.Is<List<string>>(list => list.Contains(imageKey))),
                Times.Once
            );
        }
    }
}

