using Amazon.S3;
using Amazon.S3.Model;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Riber.Application.Exceptions;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Tests;
using Riber.Infrastructure.Services.AWS;

namespace Riber.Infrastructure.Tests.Services.AWS;

public sealed class AmazonS3ServiceTests : BaseTest
{
    #region Setup

    private readonly Mock<IAmazonS3> _mockS3Client;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<AmazonS3Service>> _mockLogger;
    private readonly AmazonS3Service _service;
    private const string TestBucketName = "test-bucket";

    public AmazonS3ServiceTests()
    {
        _mockS3Client = new Mock<IAmazonS3>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<AmazonS3Service>>();

        _mockConfiguration.Setup(x => x["AWS:S3:BucketImagesName"])
            .Returns(TestBucketName);

        _service = new AmazonS3Service(
            _mockS3Client.Object,
            _mockConfiguration.Object,
            _mockLogger.Object
        );
    }

    #endregion

    #region Constructor Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Constructor should throw ArgumentNullException when configuration returns null")]
    public void Constructor_WithNullBucketName_ShouldThrowArgumentNullException()
    {
        // Arrange
        _mockConfiguration.Setup(x => x["AWS:S3:BucketImagesName"])
            .Returns((string?)null);

        // Act
        var action = () => new AmazonS3Service(
            _mockS3Client.Object,
            _mockConfiguration.Object,
            _mockLogger.Object);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("configuration");
    }

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "Constructor should throw InvalidOperationException when bucket name is empty or whitespace")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void Constructor_WithEmptyBucketName_ShouldThrowInvalidOperationException(string emptyBucketName)
    {
        // Arrange
        _mockConfiguration.Setup(x => x["AWS:S3:BucketImagesName"])
            .Returns(emptyBucketName);

        // Act
        var action = () => new AmazonS3Service(
            _mockS3Client.Object,
            _mockConfiguration.Object,
            _mockLogger.Object);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*AWS S3 bucket name is not configured*");
    }

    #endregion

    #region UploadAsync Success Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "UploadAsync should upload image successfully")]
    public async Task UploadAsync_WithValidImage_ShouldUploadSuccessfully()
    {
        // Arrange
        using var stream = new MemoryStream("Test image content"u8.ToArray());
        const string fileName = "test-image.png";
        const string contentType = "image/png";

        _mockS3Client.Setup(x => x.PutObjectAsync(
                It.IsAny<PutObjectRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutObjectResponse());

        // Act
        var action = async () => await _service.UploadAsync(stream, fileName, contentType);

        // Assert
        await action.Should().NotThrowAsync();

        _mockS3Client.Verify(x => x.PutObjectAsync(
            It.Is<PutObjectRequest>(req =>
                req.BucketName == TestBucketName &&
                req.Key == fileName &&
                req.ContentType == contentType &&
                req.ServerSideEncryptionMethod == ServerSideEncryptionMethod.AES256 &&
                req.StorageClass == S3StorageClass.Standard &&
                req.InputStream == stream
            ), It.IsAny<CancellationToken>()), Times.Once);

        VerifyLogCalled(LogLevel.Debug, "Successfully uploaded image");
    }

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "UploadAsync should handle different image types correctly")]
    [InlineData("test.jpg", "image/jpeg")]
    [InlineData("test.png", "image/png")]
    [InlineData("test.webp", "image/webp")]
    public async Task UploadAsync_WithDifferentImageTypes_ShouldUploadWithCorrectContentType(
        string fileName, string contentType)
    {
        // Arrange
        using var stream = new MemoryStream("Test content"u8.ToArray());
        _mockS3Client.Setup(x => x.PutObjectAsync(
                It.IsAny<PutObjectRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutObjectResponse());

        // Act
        await _service.UploadAsync(stream, fileName, contentType);

        // Assert
        _mockS3Client.Verify(x => x.PutObjectAsync(
            It.Is<PutObjectRequest>(req =>
                req.ContentType == contentType &&
                req.Key == fileName),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region UploadAsync Error Handling Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "UploadAsync should throw InternalException when AccessDenied")]
    public async Task UploadAsync_WhenAccessDenied_ShouldThrowInternalException()
    {
        // Arrange
        using var stream = new MemoryStream("Test content"u8.ToArray());
        var s3Exception = new AmazonS3Exception("Access Denied") { ErrorCode = "AccessDenied" };

        _mockS3Client.Setup(x => x.PutObjectAsync(
                It.IsAny<PutObjectRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(s3Exception);

        // Act
        var action = async () => await _service.UploadAsync(stream, "test.png", "image/png");

        // Assert
        await action.Should().ThrowAsync<InternalException>()
            .WithMessage(StorageErrors.AccessDenied);

        VerifyLogCalled(LogLevel.Error);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "UploadAsync should throw InternalException when bucket not found")]
    public async Task UploadAsync_WhenBucketNotFound_ShouldThrowInternalException()
    {
        // Arrange
        using var stream = new MemoryStream("Test content"u8.ToArray());
        var s3Exception = new AmazonS3Exception("Bucket not found") { ErrorCode = "NoSuchBucket" };

        _mockS3Client.Setup(x => x.PutObjectAsync(
                It.IsAny<PutObjectRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(s3Exception);

        // Act
        var action = async () => await _service.UploadAsync(stream, "test.png", "image/png");

        // Assert
        await action.Should().ThrowAsync<InternalException>()
            .WithMessage(StorageErrors.BucketNotFound);

        VerifyLogCalled(LogLevel.Error);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "UploadAsync should throw InternalException for other S3 exceptions")]
    public async Task UploadAsync_WhenOtherS3Exception_ShouldThrowInternalException()
    {
        // Arrange
        using var stream = new MemoryStream("Test content"u8.ToArray());
        var s3Exception = new AmazonS3Exception("Some S3 error") { ErrorCode = "SomeError" };

        _mockS3Client.Setup(x => x.PutObjectAsync(
                It.IsAny<PutObjectRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(s3Exception);

        // Act
        var action = async () => await _service.UploadAsync(stream, "test.png", "image/png");

        // Assert
        await action.Should().ThrowAsync<InternalException>()
            .WithMessage(StorageErrors.UploadFailed);

        VerifyLogCalled(LogLevel.Error);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "UploadAsync should throw InternalException for unexpected exceptions")]
    public async Task UploadAsync_WhenUnexpectedException_ShouldThrowInternalException()
    {
        // Arrange
        using var stream = new MemoryStream("Test content"u8.ToArray());
        var exception = new InvalidOperationException("Unexpected error");

        _mockS3Client.Setup(x => x.PutObjectAsync(
                It.IsAny<PutObjectRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var action = async () => await _service.UploadAsync(stream, "test.png", "image/png");

        // Assert
        await action.Should().ThrowAsync<InternalException>()
            .WithMessage(UnexpectedErrors.Response);

        VerifyLogCalled(LogLevel.Error);
    }

    #endregion

    #region GetImageStreamAsync Success Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "GetImageStreamAsync should return stream when image exists")]
    public async Task GetImageStreamAsync_WhenImageExists_ShouldReturnStream()
    {
        // Arrange
        const string fileName = "existing-image.png";
        var mockStream = new MemoryStream("Image content"u8.ToArray());
        var mockResponse = new GetObjectResponse { ResponseStream = mockStream };

        _mockS3Client.Setup(x => x.GetObjectAsync(
                It.IsAny<GetObjectRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _service.GetImageStreamAsync(fileName);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(mockStream);

        _mockS3Client.Verify(x => x.GetObjectAsync(
            It.Is<GetObjectRequest>(req =>
                req.BucketName == TestBucketName &&
                req.Key == fileName
            ), It.IsAny<CancellationToken>()), Times.Once);

        VerifyLogCalled(LogLevel.Debug, "Successfully retrieved image");
    }

    #endregion

    #region GetImageStreamAsync Error Handling Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "GetImageStreamAsync should throw NotFoundException when file not found")]
    public async Task GetImageStreamAsync_WhenFileNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        const string fileName = "non-existent.png";
        var s3Exception = new AmazonS3Exception("Not found") { ErrorCode = "NoSuchKey" };

        _mockS3Client.Setup(x => x.GetObjectAsync(
                It.IsAny<GetObjectRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(s3Exception);

        // Act
        var action = async () => await _service.GetImageStreamAsync(fileName);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage(StorageErrors.ImageNotFound);

        VerifyLogCalled(LogLevel.Error);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "GetImageStreamAsync should throw InternalException when AccessDenied")]
    public async Task GetImageStreamAsync_WhenAccessDenied_ShouldThrowInternalException()
    {
        // Arrange
        const string fileName = "restricted-file.png";
        var s3Exception = new AmazonS3Exception("Access Denied") { ErrorCode = "AccessDenied" };

        _mockS3Client.Setup(x => x.GetObjectAsync(
                It.IsAny<GetObjectRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(s3Exception);

        // Act
        var action = async () => await _service.GetImageStreamAsync(fileName);

        // Assert
        await action.Should().ThrowAsync<InternalException>()
            .WithMessage(StorageErrors.RetrieveAccessDenied);

        VerifyLogCalled(LogLevel.Error);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "GetImageStreamAsync should throw InternalException for other S3 exceptions")]
    public async Task GetImageStreamAsync_WhenOtherS3Exception_ShouldThrowInternalException()
    {
        // Arrange
        const string fileName = "test-file.png";
        var s3Exception = new AmazonS3Exception("Some error") { ErrorCode = "SomeError" };

        _mockS3Client.Setup(x => x.GetObjectAsync(
                It.IsAny<GetObjectRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(s3Exception);

        // Act
        var action = async () => await _service.GetImageStreamAsync(fileName);

        // Assert
        await action.Should().ThrowAsync<InternalException>()
            .WithMessage(StorageErrors.RetrieveFailed);

        VerifyLogCalled(LogLevel.Error);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "GetImageStreamAsync should throw InternalException for unexpected exceptions")]
    public async Task GetImageStreamAsync_WhenUnexpectedException_ShouldThrowInternalException()
    {
        // Arrange
        const string fileName = "test-file.png";
        var exception = new InvalidOperationException("Unexpected error");

        _mockS3Client.Setup(x => x.GetObjectAsync(
                It.IsAny<GetObjectRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var action = async () => await _service.GetImageStreamAsync(fileName);

        // Assert
        await action.Should().ThrowAsync<InternalException>()
            .WithMessage(UnexpectedErrors.Response);

        VerifyLogCalled(LogLevel.Error);
    }

    #endregion

    #region DeleteAllAsync Success Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "DeleteAllAsync should delete multiple files successfully")]
    public async Task DeleteAllAsync_WithMultipleFiles_ShouldDeleteAll()
    {
        // Arrange
        var fileNames = new List<string> { "file1.jpg", "file2.png", "file3.webp" };
        var deletedObjects = fileNames.Select(key => new DeletedObject { Key = key }).ToList();

        _mockS3Client.Setup(x => x.DeleteObjectsAsync(
                It.IsAny<DeleteObjectsRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteObjectsResponse
            {
                DeletedObjects = deletedObjects, DeleteErrors = []
            });

        // Act
        var result = (await _service.DeleteAllAsync(fileNames)).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(fileNames);

        _mockS3Client.Verify(x => x.DeleteObjectsAsync(
            It.Is<DeleteObjectsRequest>(req =>
                req.BucketName == TestBucketName &&
                req.Objects.Count == 3
            ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "DeleteAllAsync should delete single file")]
    public async Task DeleteAllAsync_WithSingleFile_ShouldDelete()
    {
        // Arrange
        var fileNames = new List<string> { "single-file.png" };
        var deletedObjects = new List<DeletedObject> { new() { Key = "single-file.png" } };

        _mockS3Client.Setup(x => x.DeleteObjectsAsync(
                It.IsAny<DeleteObjectsRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteObjectsResponse
            {
                DeletedObjects = deletedObjects, DeleteErrors = []
            });

        // Act
        var result = (await _service.DeleteAllAsync(fileNames)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain("single-file.png");
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "DeleteAllAsync should return only successfully deleted files")]
    public async Task DeleteAllAsync_WhenSomeFilesFailToDelete_ShouldReturnOnlySuccessful()
    {
        // Arrange
        var fileNames = new List<string> { "file1.jpg", "file2.png", "file3.webp" };
        var deletedObjects = new List<DeletedObject>
        {
            new() { Key = "file1.jpg" }, new() { Key = "file3.webp" }
        };
        var deleteErrors = new List<DeleteError> { new() { Key = "file2.png", Message = "Access Denied" } };

        _mockS3Client.Setup(x => x.DeleteObjectsAsync(
                It.IsAny<DeleteObjectsRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteObjectsResponse { DeletedObjects = deletedObjects, DeleteErrors = deleteErrors });

        // Act
        var result = (await _service.DeleteAllAsync(fileNames)).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain("file1.jpg");
        result.Should().Contain("file3.webp");
        result.Should().NotContain("file2.png");
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "DeleteAllAsync should handle empty list")]
    public async Task DeleteAllAsync_WithEmptyList_ShouldReturnEmptyList()
    {
        // Arrange
        _mockS3Client.Setup(x => x.DeleteObjectsAsync(
                It.IsAny<DeleteObjectsRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteObjectsResponse
            {
                DeletedObjects = [], DeleteErrors = []
            });

        // Act
        var result = (await _service.DeleteAllAsync([])).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "DeleteAllAsync should handle different file types")]
    [InlineData("file1.jpg", "file2.png")]
    [InlineData("doc.pdf", "image.gif", "photo.jpeg")]
    public async Task DeleteAllAsync_WithDifferentFileTypes_ShouldDeleteAll(params string[] fileNames)
    {
        // Arrange
        var fileList = fileNames.ToList();
        var deletedObjects = fileList.Select(key => new DeletedObject { Key = key }).ToList();

        _mockS3Client.Setup(x => x.DeleteObjectsAsync(
                It.IsAny<DeleteObjectsRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteObjectsResponse
            {
                DeletedObjects = deletedObjects, DeleteErrors = []
            });

        // Act
        var result = (await _service.DeleteAllAsync(fileList)).ToList();

        // Assert
        result.Should().HaveCount(fileNames.Length);
        result.Should().BeEquivalentTo(fileNames);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "DeleteAllAsync should log deletion summary")]
    public async Task DeleteAllAsync_ShouldLogDeletionSummary()
    {
        // Arrange
        var fileNames = new List<string> { "file1.jpg", "file2.png" };
        var deletedObjects = fileNames.Select(key => new DeletedObject { Key = key }).ToList();

        _mockS3Client.Setup(x => x.DeleteObjectsAsync(
                It.IsAny<DeleteObjectsRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteObjectsResponse
            {
                DeletedObjects = deletedObjects, DeleteErrors = []
            });

        // Act
        await _service.DeleteAllAsync(fileNames);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Deleted")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region DeleteAllAsync Error Handling Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "DeleteAllAsync should throw InternalException on S3 error")]
    public async Task DeleteAllAsync_WhenS3Exception_ShouldThrowInternalException()
    {
        // Arrange
        var fileNames = new List<string> { "file1.jpg", "file2.png" };
        var s3Exception = new AmazonS3Exception("Connection failed") { ErrorCode = "ServiceUnavailable" };

        _mockS3Client.Setup(x => x.DeleteObjectsAsync(
                It.IsAny<DeleteObjectsRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(s3Exception);

        // Act
        var action = async () => await _service.DeleteAllAsync(fileNames);

        // Assert
        await action.Should().ThrowAsync<InternalException>()
            .WithMessage(UnexpectedErrors.Response);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "DeleteAllAsync should throw InternalException on unexpected error")]
    public async Task DeleteAllAsync_WhenUnexpectedErrorOccurs_ShouldThrowInternalException()
    {
        // Arrange
        var fileNames = new List<string> { "file1.jpg" };
        var exception = new InvalidOperationException("Unexpected error");

        _mockS3Client.Setup(x => x.DeleteObjectsAsync(
                It.IsAny<DeleteObjectsRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var action = async () => await _service.DeleteAllAsync(fileNames);

        // Assert
        await action.Should().ThrowAsync<InternalException>()
            .WithMessage(UnexpectedErrors.Response);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "DeleteAllAsync should log error when exception occurs")]
    public async Task DeleteAllAsync_WhenExceptionOccurs_ShouldLogError()
    {
        // Arrange
        var fileNames = new List<string> { "file1.jpg" };
        var exception = new AmazonS3Exception("S3 Error");

        _mockS3Client.Setup(x => x.DeleteObjectsAsync(
                It.IsAny<DeleteObjectsRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        try
        {
            await _service.DeleteAllAsync(fileNames);
        }
        catch
        {
            // Expected
        }

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region Helper Methods

    private void VerifyLogCalled(LogLevel level, string? message = null)
    {
        _mockLogger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => message == null || v.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    #endregion
}