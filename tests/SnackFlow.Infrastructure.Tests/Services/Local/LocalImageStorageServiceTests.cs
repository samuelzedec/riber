using System.Text;
using FluentAssertions;
using SnackFlow.Application.Exceptions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Tests;
using SnackFlow.Infrastructure.Services.Local;

namespace SnackFlow.Infrastructure.Tests.Services.Local;

public sealed class LocalImageStorageServiceTests : BaseTest, IDisposable
{
    #region Setup

    private readonly LocalImageStorageService _service = new();
    private readonly List<string> _createdFiles = [];

    #endregion

    #region Success Tests

    [Fact(DisplayName = "UploadAsync should save image and return unique file name")]
    public async Task UploadAsync_WithValidImage_ShouldReturnUniqueFileName()
    {
        // Arrange
        using var stream = new MemoryStream("Fake Image"u8.ToArray());
        const string fileName = "fake-image.png";
        const string contentType = "image/png";
        
        // Act
        var result = await _service.UploadAsync(stream, fileName, contentType);
        _createdFiles.Add(result);
        
        // Assert
        result.Should().NotBeNullOrWhiteSpace();
        result.Should().EndWith(".png");
        result.Should().MatchRegex(@"^[\da-f]{8}-[\da-f]{4}-7[\da-f]{3}-[89ab][\da-f]{3}-[\da-f]{12}\.png$");
    }

    [Fact(DisplayName = "GetImageStreamAsync should return stream with correct content")]
    public async Task GetImageStreamAsync_WhenImageExists_ShouldReturnCorrectContent()
    {
        // Arrange
        var originalContent = "Test Image Content"u8.ToArray();
        using var uploadStream = new MemoryStream(originalContent);
        
        var fileName = await _service.UploadAsync(uploadStream, "test.jpg", "image/jpeg");
        _createdFiles.Add(fileName);

        // Act
        await using var resultStream = await _service.GetImageStreamAsync(fileName);
        using var memoryStream = new MemoryStream();
        await resultStream.CopyToAsync(memoryStream);
        
        // Assert
        resultStream.Should().NotBeNull();
        resultStream.CanRead.Should().BeTrue();
        memoryStream.ToArray().Should().BeEquivalentTo(originalContent);
    }

    [Fact(DisplayName = "GetImageStreamAsync should return stream with async file access")]
    public async Task GetImageStreamAsync_ShouldConfigureStreamForAsyncAccess()
    {
        // Arrange
        using var uploadStream = new MemoryStream("Test content"u8.ToArray());
        var fileName = await _service.UploadAsync(uploadStream, "async-test.png", "image/png");
        _createdFiles.Add(fileName);

        // Act
        await using var stream = await _service.GetImageStreamAsync(fileName);
        
        // Assert
        stream.Should().BeOfType<FileStream>();
        var fileStream = (FileStream)stream;
        fileStream.IsAsync.Should().BeTrue();
    }

    [Fact(DisplayName = "DeleteAsync should remove image completely")]
    public async Task DeleteAsync_WhenImageExists_ShouldRemoveFile()
    {
        // Arrange
        using var stream = new MemoryStream("Content to delete"u8.ToArray());
        var fileName = await _service.UploadAsync(stream, "delete-test.png", "image/png");

        // Verify the file exists first
        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "storage", "images", fileName);
        File.Exists(imagePath).Should().BeTrue();

        // Act
        var deleteAction = async () => await _service.DeleteAsync(fileName);
        
        // Assert
        await deleteAction.Should().NotThrowAsync();
        File.Exists(imagePath).Should().BeFalse();
    }

    #endregion

    #region Error Tests

    [Fact(DisplayName = "UploadAsync should throw BadRequestException for invalid content type")]
    public async Task UploadAsync_WithInvalidContentType_ShouldThrowBadRequestException()
    {
        // Arrange
        using var stream = new MemoryStream("fake content"u8.ToArray());
        const string fileName = "test.txt";
        const string invalidContentType = "text/plain";

        // Act
        var action = async () => await _service.UploadAsync(stream, fileName, invalidContentType);
        
        // Assert
        await action.Should().ThrowAsync<BadRequestException>()
            .WithMessage(ErrorMessage.Image.IsInvalid);
    }

    [Theory(DisplayName = "UploadAsync should throw BadRequestException for various invalid content types")]
    [InlineData("text/plain")]
    [InlineData("application/pdf")]
    [InlineData("video/mp4")]
    [InlineData("application/json")]
    [InlineData("")]
    public async Task UploadAsync_WithVariousInvalidContentTypes_ShouldThrow(string invalidContentType)
    {
        // Arrange
        using var stream = new MemoryStream("content"u8.ToArray());

        // Act & Assert
        var action = async () => await _service.UploadAsync(stream, "test.file", invalidContentType);
        await action.Should().ThrowAsync<BadRequestException>();
    }

    [Fact(DisplayName = "GetImageStreamAsync should throw NotFoundException when image does not exist")]
    public async Task GetImageStreamAsync_WhenImageDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        const string nonExistentFileName = "non-existent-file.png";

        // Act & Assert
        var action = async () => await _service.GetImageStreamAsync(nonExistentFileName);
        
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage(ErrorMessage.Image.NoExists);
    }

    [Fact(DisplayName = "DeleteAsync should throw NotFoundException when image does not exist")]
    public async Task DeleteAsync_WhenImageDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        const string nonExistentFileName = "non-existent-file.png";

        // Act & Assert
        var action = async () => await _service.DeleteAsync(nonExistentFileName);
        
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage(ErrorMessage.Image.NoExists);
    }

    [Theory(DisplayName = "GetImageStreamAsync should handle various non-existent files")]
    [InlineData("")]
    [InlineData("invalid-guid.png")]
    [InlineData("../../../etc/passwd")]
    [InlineData("C:\\Windows\\System32\\config\\SAM")]
    public async Task GetImageStreamAsync_WithVariousInvalidFileNames_ShouldThrow(string invalidFileName)
    {
        // Act & Assert
        var action = async () => await _service.GetImageStreamAsync(invalidFileName);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region Integration Tests

    [Fact(DisplayName = "Complete workflow should work end-to-end")]
    public async Task CompleteWorkflow_UploadGetDelete_ShouldWorkEndToEnd()
    {
        // Arrange
        var testContent = "Complete workflow test content"u8.ToArray();
        using var uploadStream = new MemoryStream(testContent);

        // Act & Assert - Upload
        var fileName = await _service.UploadAsync(uploadStream, "workflow-test.jpg", "image/jpeg");
        fileName.Should().NotBeNullOrWhiteSpace();

        // Act & Assert - Get
        await using var retrievedStream = await _service.GetImageStreamAsync(fileName);
        using var contentStream = new MemoryStream();
        await retrievedStream.CopyToAsync(contentStream);
        contentStream.ToArray().Should().BeEquivalentTo(testContent);

        // Act & Assert - Delete
        await _service.DeleteAsync(fileName);
        
        // Verify deletion
        var getAfterDelete = async () => await _service.GetImageStreamAsync(fileName);
        await getAfterDelete.Should().ThrowAsync<NotFoundException>();
    }

    [Fact(DisplayName = "Multiple uploads should generate unique file names")]
    public async Task MultipleUploads_ShouldGenerateUniqueFileNames()
    {
        // Arrange
        var fileNames = new List<string>();
        
        // Act
        for (int i = 0; i < 5; i++)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes($"Concurrent content {i}"));
            var fileName = await _service.UploadAsync(stream, "test.png", "image/png");
            fileNames.Add(fileName);
            _createdFiles.Add(fileName);
        }
        
        // Assert
        fileNames.Should().OnlyHaveUniqueItems();
        fileNames.Should().AllSatisfy(name => name.Should().EndWith(".png"));
        fileNames.Should().HaveCount(5);
    }

    [Fact(DisplayName = "Should handle concurrent uploads correctly")]
    public async Task ConcurrentUploads_ShouldGenerateUniqueFileNames()
    {
        // Arrange
        var tasks = new List<Task<string>>();
        
        // Act
        for (int i = 0; i < 10; i++)
        {
            var content = Encoding.UTF8.GetBytes($"Concurrent content {i}");
            var task = Task.Run(async () =>
            {
                using var stream = new MemoryStream(content);
                return await _service.UploadAsync(stream, $"concurrent-{i}.png", "image/png");
            });
            tasks.Add(task);
        }
        
        var fileNames = await Task.WhenAll(tasks);
        _createdFiles.AddRange(fileNames);
        
        // Assert
        fileNames.Should().OnlyHaveUniqueItems();
        fileNames.Should().HaveCount(10);
    }

    [Fact(DisplayName = "Should handle large file upload")]
    public async Task UploadAsync_WithLargeFile_ShouldWork()
    {
        // Arrange
        var largeContent = new byte[1024 * 1024];
        new Random().NextBytes(largeContent);
        using var stream = new MemoryStream(largeContent);

        // Act
        var fileName = await _service.UploadAsync(stream, "large-file.png", "image/png");
        _createdFiles.Add(fileName);
        
        // Assert
        fileName.Should().NotBeNullOrWhiteSpace();
        
        // Verify content
        await using var retrievedStream = await _service.GetImageStreamAsync(fileName);
        using var resultStream = new MemoryStream();
        await retrievedStream.CopyToAsync(resultStream);
        resultStream.ToArray().Should().BeEquivalentTo(largeContent);
    }

    [Fact(DisplayName = "Should handle empty file upload")]
    public async Task UploadAsync_WithEmptyFile_ShouldWork()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act
        var fileName = await _service.UploadAsync(stream, "empty.png", "image/png");
        _createdFiles.Add(fileName);
        
        // Assert
        fileName.Should().NotBeNullOrWhiteSpace();
        
        // Verify empty content
        await using var retrievedStream = await _service.GetImageStreamAsync(fileName);
        retrievedStream.Length.Should().Be(0);
    }

    #endregion

    #region Edge Cases

    [Fact(DisplayName = "Should handle file names with special characters")]
    public async Task UploadAsync_WithSpecialCharactersInFileName_ShouldWork()
    {
        // Arrange
        using var stream = new MemoryStream("Test content"u8.ToArray());
        const string fileName = "test file with spaces & special chars!@#$%.png";

        // Act
        var result = await _service.UploadAsync(stream, fileName, "image/png");
        _createdFiles.Add(result);
        
        // Assert
        result.Should().NotBeNullOrWhiteSpace();
        result.Should().EndWith(".png");
        
        // Verify the file can be retrieved
        await using var retrievedStream = await _service.GetImageStreamAsync(result);
        retrievedStream.Should().NotBeNull();
    }

    [Fact(DisplayName = "Should handle files without extension")]
    public async Task UploadAsync_WithFileNameWithoutExtension_ShouldWork()
    {
        // Arrange
        using var stream = new MemoryStream("Test content"u8.ToArray());
        const string fileName = "test-file";

        // Act
        var result = await _service.UploadAsync(stream, fileName, "image/png");
        _createdFiles.Add(result);
        
        // Assert
        result.Should().NotBeNullOrWhiteSpace();
        result.Should().NotEndWith(".");
        
        // Should still be able to retrieve
        await using var retrievedStream = await _service.GetImageStreamAsync(result);
        retrievedStream.Should().NotBeNull();
    }

    #endregion

    #region Cleanup

    public void Dispose()
    {
        foreach (var fileName in _createdFiles)
        {
            try
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "storage", "images", fileName);
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    #endregion
}