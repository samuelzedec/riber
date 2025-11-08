using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz;
using Riber.Application.Abstractions.Services;
using Riber.Domain.Entities.Catalog;
using Riber.Domain.Repositories;
using Riber.Infrastructure.BackgroundJobs;

namespace Riber.Infrastructure.Tests.BackgroundJobs;

public class CleanupImageBucketJobTests
{
    private readonly Mock<IImageStorageService> _mockImageStorageService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ILogger<CleanupImageBucketJob>> _mockLogger;
    private readonly Mock<IJobExecutionContext> _mockJobContext;
    private readonly CleanupImageBucketJob _job;
    private readonly Faker<Image> _imageFaker;

    public CleanupImageBucketJobTests()
    {
        _mockImageStorageService = new Mock<IImageStorageService>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<CleanupImageBucketJob>>();
        _mockJobContext = new Mock<IJobExecutionContext>();

        _mockUnitOfWork.Setup(x => x.Products).Returns(_mockProductRepository.Object);

        _job = new CleanupImageBucketJob(
            _mockImageStorageService.Object,
            _mockUnitOfWork.Object,
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
    [Fact(DisplayName = "Execute should complete without processing when there are no unused images")]
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
            x => x.DeleteAllAsync(It.IsAny<List<string>>()),
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
    [Fact(DisplayName = "Execute should delete all unused images successfully")]
    public async Task Execute_WhenUnusedImagesExist_ShouldDeleteAllSuccessfully()
    {
        // Arrange
        var unusedImages = _imageFaker.Generate(5);
        var deletedKeys = unusedImages.Select(x => x.ToString()).ToList();

        _mockProductRepository
            .Setup(x => x.GetUnusedImagesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(unusedImages);

        _mockImageStorageService
            .Setup(x => x.DeleteAllAsync(It.IsAny<List<string>>()))
            .ReturnsAsync(deletedKeys.AsEnumerable());

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockImageStorageService.Verify(
            x => x.DeleteAllAsync(It.Is<List<string>>(list => list.Count == 5)),
            Times.Once
        );

        _mockProductRepository.Verify(
            x => x.DeleteImage(It.IsAny<Image>()),
            Times.Exactly(5)
        );

        _mockUnitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Execute should log information when starting cleanup with image count")]
    public async Task Execute_WhenStartingCleanup_ShouldLogImageCount()
    {
        // Arrange
        var unusedImages = _imageFaker.Generate(3);
        var deletedKeys = unusedImages.Select(x => x.ToString()).ToList();

        _mockProductRepository
            .Setup(x => x.GetUnusedImagesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(unusedImages);

        _mockImageStorageService
            .Setup(x => x.DeleteAllAsync(It.IsAny<List<string>>()))
            .ReturnsAsync(deletedKeys.AsEnumerable());

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

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
    [Fact(DisplayName = "Execute should mark only successfully deleted images for deletion")]
    public async Task Execute_WhenPartialDeletion_ShouldMarkOnlySuccessfulOnes()
    {
        // Arrange
        var unusedImages = _imageFaker.Generate(3);
        var deletedKeys = new List<string> { unusedImages[0].ToString(), unusedImages[2].ToString() };

        _mockProductRepository
            .Setup(x => x.GetUnusedImagesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(unusedImages);

        _mockImageStorageService
            .Setup(x => x.DeleteAllAsync(It.IsAny<List<string>>()))
            .ReturnsAsync(deletedKeys.AsEnumerable());

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockProductRepository.Verify(
            x => x.DeleteImage(unusedImages[0]),
            Times.Once
        );

        _mockProductRepository.Verify(
            x => x.DeleteImage(unusedImages[1]),
            Times.Never
        );

        _mockProductRepository.Verify(
            x => x.DeleteImage(unusedImages[2]),
            Times.Once
        );

        _mockUnitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Execute should log error when deletion fails")]
    public async Task Execute_WhenDeletionFails_ShouldLogError()
    {
        // Arrange
        var unusedImages = _imageFaker.Generate(1);
        var expectedException = new Exception("Error deleting from bucket");

        _mockProductRepository
            .Setup(x => x.GetUnusedImagesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(unusedImages);

        _mockImageStorageService
            .Setup(x => x.DeleteAllAsync(It.IsAny<List<string>>()))
            .ThrowsAsync(expectedException);

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to delete images from storage service")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );

        _mockProductRepository.Verify(
            x => x.DeleteImage(It.IsAny<Image>()),
            Times.Never
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Execute should pass correct CancellationToken when fetching unused images")]
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
    [Fact(DisplayName = "Execute should not save changes when no images are deleted successfully")]
    public async Task Execute_WhenNoImagesDeletedSuccessfully_ShouldNotSaveChanges()
    {
        // Arrange
        var unusedImages = _imageFaker.Generate(2);
        var emptyDeletedKeys = new List<string>();

        _mockProductRepository
            .Setup(x => x.GetUnusedImagesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(unusedImages);

        _mockImageStorageService
            .Setup(x => x.DeleteAllAsync(It.IsAny<List<string>>()))
            .ReturnsAsync(emptyDeletedKeys.AsEnumerable());

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockProductRepository.Verify(
            x => x.DeleteImage(It.IsAny<Image>()),
            Times.Never
        );

        _mockUnitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}