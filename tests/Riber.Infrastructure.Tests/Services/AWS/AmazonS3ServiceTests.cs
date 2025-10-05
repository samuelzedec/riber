// using System.Text;
// using Amazon.S3;
// using Amazon.S3.Model;
// using FluentAssertions;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;
// using Moq;
// using Riber.Application.Exceptions;
//
// using Riber.Domain.Tests;
// using Riber.Infrastructure.Services.AWS;
//
// namespace Riber.Infrastructure.Tests.Services.AWS;
//
// public sealed class AmazonS3ServiceTests : BaseTest
// {
//     #region Setup
//
//     private readonly Mock<IAmazonS3> _mockS3Client;
//     private readonly Mock<IConfiguration> _mockConfiguration;
//     private readonly Mock<ILogger<AmazonS3Service>> _mockLogger;
//     private readonly AmazonS3Service _service;
//     private const string TestBucketName = "test-bucket";
//
//     public AmazonS3ServiceTests()
//     {
//         _mockS3Client = new Mock<IAmazonS3>();
//         _mockConfiguration = new Mock<IConfiguration>();
//         _mockLogger = new Mock<ILogger<AmazonS3Service>>();
//
//         _mockConfiguration.Setup(x => x["AWS:S3:BucketImagesName"])
//             .Returns(TestBucketName);
//
//         _service = new AmazonS3Service(
//             _mockS3Client.Object,
//             _mockConfiguration.Object,
//             _mockLogger.Object
//         );
//     }
//
//     #endregion
//
//     #region Constructor Tests
//
//     [Fact(DisplayName = "Constructor should throw InvalidOperationException when bucket name is not configured")]
//     public void Constructor_WithNullBucketName_ShouldThrowInvalidOperationException()
//     {
//         // Arrange
//         _mockConfiguration.Setup(x => x["AWS:S3:BucketImagesName"])
//             .Returns((string?)null);
//
//         // Act
//         var action = () => new AmazonS3Service(_mockS3Client.Object, _mockConfiguration.Object, _mockLogger.Object);
//
//         // Assert
//         action.Should().Throw<ArgumentNullException>()
//             .WithMessage("Configuration cannot be null (Parameter 'configuration')");
//     }
//
//     [Theory(DisplayName = "Constructor should throw InvalidOperationException when bucket name is empty or whitespace")]
//     [InlineData("")]
//     [InlineData("   ")]
//     [InlineData("\t")]
//     public void Constructor_WithEmptyBucketName_ShouldThrowInvalidOperationException(string emptyBucketName)
//     {
//         // Arrange
//         _mockConfiguration.Setup(x => x["AWS:S3:BucketImagesName"])
//             .Returns(emptyBucketName);
//
//         // Act
//         var action = () => new AmazonS3Service(_mockS3Client.Object, _mockConfiguration.Object, _mockLogger.Object);
//
//         // Assert
//         action.Should().Throw<InvalidOperationException>()
//             .WithMessage("*AWS S3 bucket name is not configured*");
//     }
//
//     #endregion
//
//     #region UploadAsync Success Tests
//
//     [Fact(DisplayName = "UploadAsync should upload image successfully and return unique file name")]
//     public async Task UploadAsync_WithValidImage_ShouldReturnUniqueFileName()
//     {
//         // Arrange
//         using var stream = new MemoryStream("Test image content"u8.ToArray());
//         const string fileName = "test-image.png";
//         const string contentType = "image/png";
//
//         _mockS3Client.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(new PutObjectResponse());
//
//         // Act
//         var result = await _service.UploadAsync(stream, fileName, contentType);
//
//         // Assert
//         result.Should().NotBeNullOrWhiteSpace();
//         result.Should().EndWith(".png");
//         result.Should().MatchRegex(@"^[\da-f]{8}-[\da-f]{4}-7[\da-f]{3}-[89ab][\da-f]{3}-[\da-f]{12}\.png$");
//
//         _mockS3Client.Verify(x => x.PutObjectAsync(
//             It.Is<PutObjectRequest>(req =>
//                 req.BucketName == TestBucketName &&
//                 req.Key == result &&
//                 req.ContentType == contentType &&
//                 req.ServerSideEncryptionMethod == ServerSideEncryptionMethod.AES256 &&
//                 req.StorageClass == S3StorageClass.Standard &&
//                 req.InputStream == stream
//             ), It.IsAny<CancellationToken>()), Times.Once);
//
//         VerifyLogCalled(LogLevel.Debug, "Successfully uploaded image");
//     }
//
//     [Theory(DisplayName = "UploadAsync should handle different image types correctly")]
//     [InlineData("test.jpg", "image/jpeg", ".jpg")]
//     [InlineData("test.JPEG", "image/jpeg", ".jpeg")]
//     [InlineData("test.png", "image/png", ".png")]
//     [InlineData("test.webp", "image/webp", ".webp")]
//     public async Task UploadAsync_WithDifferentImageTypes_ShouldPreserveExtension(
//         string fileName, string contentType, string expectedExtension)
//     {
//         // Arrange
//         using var stream = new MemoryStream("Test content"u8.ToArray());
//         _mockS3Client.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(new PutObjectResponse());
//
//         // Act
//         var result = await _service.UploadAsync(stream, fileName, contentType);
//
//         // Assert
//         result.Should().EndWith(expectedExtension);
//
//         _mockS3Client.Verify(x => x.PutObjectAsync(
//             It.Is<PutObjectRequest>(req => req.ContentType == contentType),
//             It.IsAny<CancellationToken>()), Times.Once);
//     }
//
//     #endregion
//
//     #region UploadAsync Validation Tests
//
//     [Theory(DisplayName = "UploadAsync should throw ArgumentException for invalid file names")]
//     [InlineData("")]
//     [InlineData("   ")]
//     [InlineData("\t")]
//     public async Task UploadAsync_WithInvalidFileName_ShouldThrowArgumentException(string invalidFileName)
//     {
//         // Arrange
//         using var stream = new MemoryStream("content"u8.ToArray());
//
//         // Act
//         var action = async () => await _service.UploadAsync(stream, invalidFileName, "image/png");
//
//         // Assert
//         await action.Should().ThrowAsync<ArgumentException>()
//             .WithParameterName("fileName");
//     }
//
//     [Theory(DisplayName = "UploadAsync should throw ArgumentException for invalid content types")]
//     [InlineData("")]
//     [InlineData("   ")]
//     public async Task UploadAsync_WithInvalidContentType_ShouldThrowArgumentException(string invalidContentType)
//     {
//         // Arrange
//         using var stream = new MemoryStream("content"u8.ToArray());
//
//         // Act
//         var action = async () => await _service.UploadAsync(stream, "test.png", invalidContentType!);
//
//         // Assert
//         await action.Should().ThrowAsync<ArgumentException>()
//             .WithParameterName("contentType");
//     }
//
//     [Fact(DisplayName = "UploadAsync should throw BadRequestException for invalid image content type")]
//     public async Task UploadAsync_WithNonImageContentType_ShouldThrowBadRequestException()
//     {
//         // Arrange
//         using var stream = new MemoryStream("Not an image"u8.ToArray());
//
//         // Act
//         var action = async () => await _service.UploadAsync(stream, "test.txt", "text/plain");
//
//         // Assert
//         await action.Should().ThrowAsync<BadRequestException>()
//             .WithMessage(ErrorMessage.Image.IsInvalid);
//
//         _mockS3Client.Verify(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Never);
//     }
//
//     #endregion
//
//     #region UploadAsync Error Handling Tests
//
//     [Fact(DisplayName = "UploadAsync should handle AccessDenied exception")]
//     public async Task UploadAsync_WhenAccessDenied_ShouldThrowInternalException()
//     {
//         // Arrange
//         using var stream = new MemoryStream("Test content"u8.ToArray());
//         var s3Exception = new AmazonS3Exception("Access Denied") { ErrorCode = "AccessDenied" };
//
//         _mockS3Client.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
//             .ThrowsAsync(s3Exception);
//
//         // Act & Assert
//         var action = async () => await _service.UploadAsync(stream, "test.png", "image/png");
//
//         await action.Should().ThrowAsync<InternalException>()
//             .WithMessage("Storage service is temporarily unavailable. Please try again later.");
//
//         VerifyLogCalled(LogLevel.Error, "Access denied when uploading image");
//     }
//
//     [Fact(DisplayName = "UploadAsync should handle NoSuchBucket exception")]
//     public async Task UploadAsync_WhenBucketNotFound_ShouldThrowInternalException()
//     {
//         // Arrange
//         using var stream = new MemoryStream("Test content"u8.ToArray());
//         var s3Exception = new AmazonS3Exception("Bucket not found") { ErrorCode = "NoSuchBucket" };
//
//         _mockS3Client.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
//             .ThrowsAsync(s3Exception);
//
//         // Act
//         var action = async () => await _service.UploadAsync(stream, "test.png", "image/png");
//
//         // Assert
//         await action.Should().ThrowAsync<InternalException>()
//             .WithMessage("Storage configuration error. Please contact support.");
//
//         VerifyLogCalled(LogLevel.Error, "S3 bucket");
//     }
//
//     [Fact(DisplayName = "UploadAsync should handle other S3 exceptions")]
//     public async Task UploadAsync_WhenOtherS3Exception_ShouldThrowInternalException()
//     {
//         // Arrange
//         using var stream = new MemoryStream("Test content"u8.ToArray());
//         var s3Exception = new AmazonS3Exception("Some S3 error") { ErrorCode = "SomeError" };
//
//         _mockS3Client.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
//             .ThrowsAsync(s3Exception);
//
//         // Act
//         var action = async () => await _service.UploadAsync(stream, "test.png", "image/png");
//
//         // Assert
//         await action.Should().ThrowAsync<InternalException>()
//             .WithMessage("Failed to upload image. Please try again later.");
//
//         VerifyLogCalled(LogLevel.Error, "S3 error when uploading image");
//     }
//
//     [Fact(DisplayName = "UploadAsync should handle unexpected exceptions")]
//     public async Task UploadAsync_WhenUnexpectedException_ShouldThrowInternalException()
//     {
//         // Arrange
//         using var stream = new MemoryStream("Test content"u8.ToArray());
//         var exception = new InvalidOperationException("Unexpected error");
//
//         _mockS3Client.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
//             .ThrowsAsync(exception);
//
//         // Act
//         var action = async () => await _service.UploadAsync(stream, "test.png", "image/png");
//
//         // Assert
//         await action.Should().ThrowAsync<InternalException>()
//             .WithMessage("An unexpected error occurred while uploading the image.");
//
//         VerifyLogCalled(LogLevel.Error, "Unexpected error when uploading image");
//     }
//
//     #endregion
//
//     #region GetImageStreamAsync Success Tests
//
//     [Fact(DisplayName = "GetImageStreamAsync should return stream when image exists")]
//     public async Task GetImageStreamAsync_WhenImageExists_ShouldReturnStream()
//     {
//         // Arrange
//         const string fileName = "existing-image.png";
//         var mockStream = new MemoryStream("Image content"u8.ToArray());
//         var mockResponse = new GetObjectResponse { ResponseStream = mockStream };
//
//         _mockS3Client.Setup(x => x.GetObjectAsync(It.IsAny<GetObjectRequest>(), CancellationToken.None))
//             .ReturnsAsync(mockResponse);
//
//         // Act
//         var result = await _service.GetImageStreamAsync(fileName);
//
//         // Assert
//         result.Should().NotBeNull();
//         result.Should().BeSameAs(mockStream);
//
//         _mockS3Client.Verify(x => x.GetObjectAsync(
//             It.Is<GetObjectRequest>(req =>
//                 req.BucketName == TestBucketName &&
//                 req.Key == fileName
//             ), It.IsAny<CancellationToken>()), Times.Once);
//
//         VerifyLogCalled(LogLevel.Debug, "Successfully retrieved image");
//     }
//
//     #endregion
//
//     #region GetImageStreamAsync Validation Tests
//
//     [Theory(DisplayName = "GetImageStreamAsync should throw ArgumentException for invalid file names")]
//     [InlineData("")]
//     [InlineData("   ")]
//     [InlineData("\t")]
//     public async Task GetImageStreamAsync_WithInvalidFileName_ShouldThrowArgumentException(string invalidFileName)
//     {
//         // Act
//         var action = async () => await _service.GetImageStreamAsync(invalidFileName);
//
//         // Assert
//         await action.Should().ThrowAsync<ArgumentException>()
//             .WithParameterName("fileName");
//     }
//
//     #endregion
//
//     #region GetImageStreamAsync Error Handling Tests
//
//     [Fact(DisplayName = "GetImageStreamAsync should throw NotFoundException when file not found")]
//     public async Task GetImageStreamAsync_WhenFileNotFound_ShouldThrowNotFoundException()
//     {
//         // Arrange
//         const string fileName = "non-existent.png";
//         var s3Exception = new AmazonS3Exception("Not found") { ErrorCode = "NoSuchKey" };
//
//         _mockS3Client.Setup(x => x.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
//             .ThrowsAsync(s3Exception);
//
//         // Act
//         var action = async () => await _service.GetImageStreamAsync(fileName);
//
//         // Assert
//         await action.Should().ThrowAsync<NotFoundException>()
//             .WithMessage(ErrorMessage.Image.NoExists);
//
//         VerifyLogCalled(LogLevel.Warning, "Image");
//         VerifyLogCalled(LogLevel.Warning, "not found");
//     }
//
//     [Fact(DisplayName = "GetImageStreamAsync should handle AccessDenied exception")]
//     public async Task GetImageStreamAsync_WhenAccessDenied_ShouldThrowInternalException()
//     {
//         // Arrange
//         const string fileName = "restricted-file.png";
//         var s3Exception = new AmazonS3Exception("Access Denied") { ErrorCode = "AccessDenied" };
//
//         _mockS3Client.Setup(x => x.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
//             .ThrowsAsync(s3Exception);
//
//         // Act & Assert
//         var action = async () => await _service.GetImageStreamAsync(fileName);
//
//         await action.Should().ThrowAsync<InternalException>()
//             .WithMessage("Storage service is temporarily unavailable. Please try again later.");
//
//         VerifyLogCalled(LogLevel.Error, "Access denied when retrieving image");
//     }
//
//     [Fact(DisplayName = "GetImageStreamAsync should handle other S3 exceptions")]
//     public async Task GetImageStreamAsync_WhenOtherS3Exception_ShouldThrowInternalException()
//     {
//         // Arrange
//         const string fileName = "test-file.png";
//         var s3Exception = new AmazonS3Exception("Some error") { ErrorCode = "SomeError" };
//
//         _mockS3Client.Setup(x => x.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
//             .ThrowsAsync(s3Exception);
//
//         // Act
//         var action = async () => await _service.GetImageStreamAsync(fileName);
//
//         // Assert
//         await action.Should().ThrowAsync<InternalException>()
//             .WithMessage("Failed to retrieve image. Please try again later.");
//
//         VerifyLogCalled(LogLevel.Error, "S3 error when retrieving image");
//     }
//
//     [Fact(DisplayName = "GetImageStreamAsync should handle unexpected exceptions")]
//     public async Task GetImageStreamAsync_WhenUnexpectedException_ShouldThrowInternalException()
//     {
//         // Arrange
//         const string fileName = "test-file.png";
//         var exception = new InvalidOperationException("Unexpected error");
//
//         _mockS3Client.Setup(x => x.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
//             .ThrowsAsync(exception);
//
//         // Act
//         var action = async () => await _service.GetImageStreamAsync(fileName);
//
//         // Assert
//         await action.Should().ThrowAsync<InternalException>()
//             .WithMessage("An unexpected error occurred while retrieving the image.");
//
//         VerifyLogCalled(LogLevel.Error, "Unexpected error when retrieving image");
//     }
//
//     #endregion
//
//     #region DeleteAsync Success Tests
//
//     [Fact(DisplayName = "DeleteAsync should delete image successfully")]
//     public async Task DeleteAsync_WhenImageExists_ShouldDeleteSuccessfully()
//     {
//         // Arrange
//         const string fileName = "image-to-delete.png";
//         _mockS3Client.Setup(x => x.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(new DeleteObjectResponse());
//
//         // Act
//         var action = async () => await _service.DeleteAsync(fileName);
//
//         // Assert
//         await action.Should().NotThrowAsync();
//
//         _mockS3Client.Verify(x => x.DeleteObjectAsync(
//             It.Is<DeleteObjectRequest>(req =>
//                 req.BucketName == TestBucketName &&
//                 req.Key == fileName
//             ), It.IsAny<CancellationToken>()), Times.Once);
//
//         VerifyLogCalled(LogLevel.Debug, "Successfully deleted image");
//     }
//
//     #endregion
//
//     #region DeleteAsync Validation Tests
//
//     [Theory(DisplayName = "DeleteAsync should throw ArgumentException for invalid file names")]
//     [InlineData("")]
//     [InlineData("   ")]
//     [InlineData("\t")]
//     public async Task DeleteAsync_WithInvalidFileName_ShouldThrowArgumentException(string invalidFileName)
//     {
//         // Act
//         var action = async () => await _service.DeleteAsync(invalidFileName!);
//
//         // Assert
//         await action.Should().ThrowAsync<ArgumentException>()
//             .WithParameterName("fileName");
//     }
//
//     #endregion
//
//     #region DeleteAsync Error Handling Tests
//
//     [Fact(DisplayName = "DeleteAsync should handle AccessDenied exception")]
//     public async Task DeleteAsync_WhenAccessDenied_ShouldThrowInternalException()
//     {
//         // Arrange
//         const string fileName = "restricted-file.png";
//         var s3Exception = new AmazonS3Exception("Access Denied") { ErrorCode = "AccessDenied" };
//
//         _mockS3Client.Setup(x => x.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), It.IsAny<CancellationToken>()))
//             .ThrowsAsync(s3Exception);
//
//         // Act
//         var action = async () => await _service.DeleteAsync(fileName);
//
//         // Assert
//         await action.Should().ThrowAsync<InternalException>()
//             .WithMessage("Storage service is temporarily unavailable. Please try again later.");
//
//         VerifyLogCalled(LogLevel.Error, "Access denied when deleting image");
//     }
//
//     [Fact(DisplayName = "DeleteAsync should handle other S3 exceptions")]
//     public async Task DeleteAsync_WhenOtherS3Exception_ShouldThrowInternalException()
//     {
//         // Arrange
//         const string fileName = "test-file.png";
//         var s3Exception = new AmazonS3Exception("Some error") { ErrorCode = "SomeError" };
//
//         _mockS3Client.Setup(x => x.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), It.IsAny<CancellationToken>()))
//             .ThrowsAsync(s3Exception);
//
//         // Act
//         var action = async () => await _service.DeleteAsync(fileName);
//
//         // Assert
//         await action.Should().ThrowAsync<InternalException>()
//             .WithMessage("Failed to delete image. Please try again later.");
//
//         VerifyLogCalled(LogLevel.Error, "S3 error when deleting image");
//     }
//
//     [Fact(DisplayName = "DeleteAsync should handle unexpected exceptions")]
//     public async Task DeleteAsync_WhenUnexpectedException_ShouldThrowInternalException()
//     {
//         // Arrange
//         const string fileName = "test-file.png";
//         var exception = new InvalidOperationException("Unexpected error");
//
//         _mockS3Client.Setup(x => x.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), It.IsAny<CancellationToken>()))
//             .ThrowsAsync(exception);
//
//         // Act
//         var action = async () => await _service.DeleteAsync(fileName);
//
//         // Assert
//         await action.Should().ThrowAsync<InternalException>()
//             .WithMessage("An unexpected error occurred while deleting the image.");
//
//         VerifyLogCalled(LogLevel.Error, "Unexpected error when deleting image");
//     }
//
//     #endregion
//
//     #region Integration Tests
//
//     [Fact(DisplayName = "Multiple uploads should generate unique file names")]
//     public async Task MultipleUploads_ShouldGenerateUniqueFileNames()
//     {
//         // Arrange
//         var fileNames = new List<string>();
//         _mockS3Client.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(new PutObjectResponse());
//
//         // Act
//         for (int i = 0; i < 5; i++)
//         {
//             using var stream = new MemoryStream(Encoding.UTF8.GetBytes($"Concurrent content {i}"));
//             var fileName = await _service.UploadAsync(stream, "test.png", "image/png");
//             fileNames.Add(fileName);
//         }
//
//         // Assert
//         fileNames.Should().OnlyHaveUniqueItems();
//         fileNames.Should().HaveCount(5);
//         fileNames.Should().AllSatisfy(name => name.Should().EndWith(".png"));
//
//         _mockS3Client.Verify(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(5));
//     }
//
//     [Fact(DisplayName = "Should handle concurrent operations correctly")]
//     public async Task ConcurrentOperations_ShouldNotInterfere()
//     {
//         // Arrange
//         _mockS3Client.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(new PutObjectResponse());
//
//         var tasks = new List<Task<string>>();
//
//         // Act
//         for (int i = 0; i < 10; i++)
//         {
//             var content = Encoding.UTF8.GetBytes($"Concurrent content {i}");
//             var task = Task.Run(async () =>
//             {
//                 using var stream = new MemoryStream(content);
//                 return await _service.UploadAsync(stream, $"concurrent-{i}.png", "image/png");
//             });
//             tasks.Add(task);
//         }
//
//         var fileNames = await Task.WhenAll(tasks);
//
//         // Assert
//         fileNames.Should().OnlyHaveUniqueItems();
//         fileNames.Should().HaveCount(10);
//         _mockS3Client.Verify(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(10));
//     }
//
//     #endregion
//
//     #region Helper Methods
//
//     private void VerifyLogCalled(LogLevel level, string message)
//     {
//         _mockLogger.Verify(
//             x => x.Log(
//                 level,
//                 It.IsAny<EventId>(),
//                 It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
//                 It.IsAny<Exception>(),
//                 It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
//             Times.AtLeastOnce);
//     }
//
//     #endregion
// }