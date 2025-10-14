using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz;
using Riber.Application.Abstractions.Services;
using Riber.Domain.Entities;
using Riber.Domain.Repositories;
using Riber.Infrastructure.BackgroundJobs;

namespace Riber.Infrastructure.Tests.BackgroundJobs;

public class CleanupImageBucketJobTests
{
    private readonly Mock<IImageStorageService> _mockImageStorageService;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ILogger<CleanupImageBucketJob>> _mockLogger;
    private readonly Mock<IJobExecutionContext> _mockJobContext;
    private readonly CleanupImageBucketJob _job;
    private readonly Faker<Image> _imageFaker;

    public CleanupImageBucketJobTests()
    {
        _mockImageStorageService = new Mock<IImageStorageService>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockLogger = new Mock<ILogger<CleanupImageBucketJob>>();
        _mockJobContext = new Mock<IJobExecutionContext>();

        _job = new CleanupImageBucketJob(
            _mockImageStorageService.Object,
            _mockProductRepository.Object,
            _mockLogger.Object
        );

        _imageFaker = new Faker<Image>()
            .CustomInstantiator(f => Image.Create(
                length: f.Random.Long(1_000L, 10_000_000L),
                contentType: f.PickRandom("image/png", "image/jpeg", "image/webp"),
                originalName: f.System.FileName("png"))
            );

        _mockJobContext.Setup(x => x.CancellationToken).Returns(CancellationToken.None);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should complete without processing when there are no unused images")]
    public async Task Execute_WhenNoUnusedImages_ShouldCompleteWithoutProcessing()
    {
        // Arrange
        var emptyImages = new List<Image>();
        _mockProductRepository
            .Setup(x => x.GetUnusedImagesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyImages);

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockImageStorageService.Verify(
            x => x.DeleteAsync(It.IsAny<string>()), 
            Times.Never
        );

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No unused images found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should delete all unused images successfully")]
    public async Task Execute_WhenUnusedImagesExist_ShouldDeleteAllSuccessfully()
    {
        // Arrange
        var unusedImages = _imageFaker.Generate(5);
        _mockProductRepository
            .Setup(x => x.GetUnusedImagesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(unusedImages);

        _mockImageStorageService
            .Setup(x => x.DeleteAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockImageStorageService.Verify(
            x => x.DeleteAsync(It.IsAny<string>()), 
            Times.Exactly(5)
        );

        foreach (var image in unusedImages)
        {
            _mockImageStorageService.Verify(
                x => x.DeleteAsync(image.ToString()), 
                Times.Once
            );
        }

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Image cleanup completed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should log information when starting cleanup with image count")]
    public async Task Execute_WhenStartingCleanup_ShouldLogImageCount()
    {
        // Arrange
        var unusedImages = _imageFaker.Generate(3);
        _mockProductRepository
            .Setup(x => x.GetUnusedImagesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(unusedImages);

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Starting cleanup of 3 unused images")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should continue processing even when deleting one image fails")]
    public async Task Execute_WhenDeletingOneImageFails_ShouldContinueProcessing()
    {
        // Arrange
        var unusedImages = _imageFaker.Generate(3);
        var imageWithError = unusedImages[1];

        _mockProductRepository
            .Setup(x => x.GetUnusedImagesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(unusedImages);

        _mockImageStorageService
            .Setup(x => x.DeleteAsync(imageWithError.ToString()))
            .ThrowsAsync(new Exception("Error deleting image from storage"));

        _mockImageStorageService
            .Setup(x => x.DeleteAsync(It.Is<string>(s => s != imageWithError.ToString())))
            .Returns(Task.CompletedTask);

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockImageStorageService.Verify(
            x => x.DeleteAsync(It.IsAny<string>()), 
            Times.Exactly(3)
        );

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successfully deleted: 2, Failed: 1")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should log error when image deletion fails")]
    public async Task Execute_WhenImageDeletionFails_ShouldLogError()
    {
        // Arrange
        var unusedImages = _imageFaker.Generate(1);
        var expectedException = new Exception("Error deleting from bucket");

        _mockProductRepository
            .Setup(x => x.GetUnusedImagesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(unusedImages);

        _mockImageStorageService
            .Setup(x => x.DeleteAsync(It.IsAny<string>()))
            .ThrowsAsync(expectedException);

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
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
    [Fact(DisplayName = "Should log debug for each successfully deleted image")]
    public async Task Execute_WhenImageIsDeletedSuccessfully_ShouldLogDebug()
    {
        // Arrange
        var unusedImages = _imageFaker.Generate(2);
        _mockProductRepository
            .Setup(x => x.GetUnusedImagesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(unusedImages);

        _mockImageStorageService
            .Setup(x => x.DeleteAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successfully deleted image")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(2)
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should pass correct CancellationToken when fetching unused images")]
    public async Task Execute_ShouldPassCancellationTokenToRepository()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        
        _mockJobContext.Setup(x => x.CancellationToken).Returns(cancellationToken);

        _mockProductRepository
            .Setup(x => x.GetUnusedImagesAsync(cancellationToken))
            .ReturnsAsync([]);

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockProductRepository.Verify(
            x => x.GetUnusedImagesAsync(cancellationToken), 
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should process multiple failures and count correctly")]
    public async Task Execute_WhenMultipleFailuresOccur_ShouldCountCorrectly()
    {
        // Arrange
        var unusedImages = _imageFaker.Generate(5);
        
        _mockProductRepository
            .Setup(x => x.GetUnusedImagesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(unusedImages);

        _mockImageStorageService
            .Setup(x => x.DeleteAsync(unusedImages[1].ToString()))
            .ThrowsAsync(new Exception("Error 1"));

        _mockImageStorageService
            .Setup(x => x.DeleteAsync(unusedImages[3].ToString()))
            .ThrowsAsync(new Exception("Error 2"));

        _mockImageStorageService
            .Setup(x => x.DeleteAsync(It.Is<string>(s => 
                s != unusedImages[1].ToString() && 
                s != unusedImages[3].ToString())))
            .Returns(Task.CompletedTask);

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successfully deleted: 3, Failed: 2")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to delete image")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(2)
        );
    }
}