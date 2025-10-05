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

    #region GetImageStreamAsync Validation Tests

    [Theory(DisplayName = "GetImageStreamAsync should throw ArgumentException for invalid file names")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public async Task GetImageStreamAsync_WithInvalidFileName_ShouldThrowArgumentException(string? invalidFileName)
    {
        // Act
        var action = async () => await _service.GetImageStreamAsync(invalidFileName!);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("fileName");
    }

    #endregion

    #region GetImageStreamAsync Error Handling Tests

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

    #region DeleteAsync Success Tests

    [Fact(DisplayName = "DeleteAsync should delete image successfully")]
    public async Task DeleteAsync_WhenImageExists_ShouldDeleteSuccessfully()
    {
        // Arrange
        const string fileName = "image-to-delete.png";
        _mockS3Client.Setup(x => x.DeleteObjectAsync(
                It.IsAny<DeleteObjectRequest>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteObjectResponse());

        // Act
        var action = async () => await _service.DeleteAsync(fileName);

        // Assert
        await action.Should().NotThrowAsync();

        _mockS3Client.Verify(x => x.DeleteObjectAsync(
            It.Is<DeleteObjectRequest>(req =>
                req.BucketName == TestBucketName &&
                req.Key == fileName
            ), It.IsAny<CancellationToken>()), Times.Once);

        VerifyLogCalled(LogLevel.Debug, "Successfully deleted image");
    }

    #endregion

    #region DeleteAsync Validation Tests

    [Theory(DisplayName = "DeleteAsync should throw ArgumentException for invalid file names")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public async Task DeleteAsync_WithInvalidFileName_ShouldThrowArgumentException(string? invalidFileName)
    {
        // Act
        var action = async () => await _service.DeleteAsync(invalidFileName!);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("fileName");
    }

    #endregion

    #region DeleteAsync Error Handling Tests

    [Fact(DisplayName = "DeleteAsync should throw InternalException for any exception")]
    public async Task DeleteAsync_WhenExceptionOccurs_ShouldThrowInternalException()
    {
        // Arrange
        const string fileName = "test-file.png";
        var exception = new InvalidOperationException("Unexpected error");

        _mockS3Client.Setup(x => x.DeleteObjectAsync(
                It.IsAny<DeleteObjectRequest>(), 
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var action = async () => await _service.DeleteAsync(fileName);

        // Assert
        await action.Should().ThrowAsync<InternalException>()
            .WithMessage(UnexpectedErrors.Response);

        VerifyLogCalled(LogLevel.Error);
    }

    [Fact(DisplayName = "DeleteAsync should throw InternalException for S3 exceptions")]
    public async Task DeleteAsync_WhenS3Exception_ShouldThrowInternalException()
    {
        // Arrange
        const string fileName = "test-file.png";
        var s3Exception = new AmazonS3Exception("S3 Error") { ErrorCode = "SomeError" };

        _mockS3Client.Setup(x => x.DeleteObjectAsync(
                It.IsAny<DeleteObjectRequest>(), 
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(s3Exception);

        // Act
        var action = async () => await _service.DeleteAsync(fileName);

        // Assert
        await action.Should().ThrowAsync<InternalException>()
            .WithMessage(UnexpectedErrors.Response);

        VerifyLogCalled(LogLevel.Error);
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