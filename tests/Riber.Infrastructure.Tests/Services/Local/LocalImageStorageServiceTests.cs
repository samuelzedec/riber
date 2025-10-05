using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Riber.Application.Exceptions;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Tests;
using Riber.Infrastructure.Services.Local;
using System.Text;

namespace Riber.Infrastructure.Tests.Services.Local;

public sealed class LocalImageStorageServiceTests : BaseTest, IDisposable
{
    #region Setup

    private readonly Mock<ILogger<LocalImageStorageService>> _mockLogger;
    private readonly LocalImageStorageService _service;
    private readonly string _storagePath;
    private readonly List<string> _createdFiles = new();

    public LocalImageStorageServiceTests()
    {
        _mockLogger = new Mock<ILogger<LocalImageStorageService>>();
        _service = new LocalImageStorageService(_mockLogger.Object);
        _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "storage", "images");
    }

    #endregion

    #region UploadAsync Success Tests

    [Fact(DisplayName = "UploadAsync should save file successfully")]
    public async Task UploadAsync_WithValidStream_ShouldSaveFile()
    {
        // Arrange
        var content = "Test image content"u8.ToArray();
        using var stream = new MemoryStream(content);
        const string fileName = "test-image.png";
        const string contentType = "image/png";

        // Act
        var action = async () => await _service.UploadAsync(stream, fileName, contentType);

        // Assert
        await action.Should().NotThrowAsync();

        var filePath = Path.Combine(_storagePath, fileName);
        File.Exists(filePath).Should().BeTrue();
        _createdFiles.Add(fileName);

        // Verify content
        var savedContent = await File.ReadAllBytesAsync(filePath);
        savedContent.Should().BeEquivalentTo(content);
    }

    [Theory(DisplayName = "UploadAsync should handle different file types")]
    [InlineData("test.jpg", "image/jpeg")]
    [InlineData("test.png", "image/png")]
    [InlineData("test.webp", "image/webp")]
    public async Task UploadAsync_WithDifferentFileTypes_ShouldSaveCorrectly(
        string fileName, string contentType)
    {
        // Arrange
        using var stream = new MemoryStream("Test content"u8.ToArray());

        // Act
        await _service.UploadAsync(stream, fileName, contentType);

        // Assert
        var filePath = Path.Combine(_storagePath, fileName);
        File.Exists(filePath).Should().BeTrue();
        _createdFiles.Add(fileName);
    }

    [Fact(DisplayName = "UploadAsync should overwrite existing file")]
    public async Task UploadAsync_WhenFileExists_ShouldOverwrite()
    {
        // Arrange
        const string fileName = "overwrite-test.png";
        var originalContent = "Original content"u8.ToArray();
        var newContent = "New content"u8.ToArray();

        using var stream1 = new MemoryStream(originalContent);
        await _service.UploadAsync(stream1, fileName, "image/png");

        // Act
        using var stream2 = new MemoryStream(newContent);
        await _service.UploadAsync(stream2, fileName, "image/png");

        // Assert
        var filePath = Path.Combine(_storagePath, fileName);
        var savedContent = await File.ReadAllBytesAsync(filePath);
        savedContent.Should().BeEquivalentTo(newContent);
        _createdFiles.Add(fileName);
    }

    #endregion
    
    #region GetImageStreamAsync Success Tests

    [Fact(DisplayName = "GetImageStreamAsync should return stream when file exists")]
    public async Task GetImageStreamAsync_WhenFileExists_ShouldReturnStream()
    {
        // Arrange
        var content = "Test image content"u8.ToArray();
        const string fileName = "get-test.png";
        var filePath = Path.Combine(_storagePath, fileName);
        await File.WriteAllBytesAsync(filePath, content);
        _createdFiles.Add(fileName);

        // Act
        await using var stream = await _service.GetImageStreamAsync(fileName);

        // Assert
        stream.Should().NotBeNull();
        stream.Should().BeOfType<FileStream>();
        stream.CanRead.Should().BeTrue();

        // Verify content
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        memoryStream.ToArray().Should().BeEquivalentTo(content);
    }

    [Fact(DisplayName = "GetImageStreamAsync should return async FileStream")]
    public async Task GetImageStreamAsync_ShouldReturnAsyncFileStream()
    {
        // Arrange
        const string fileName = "async-test.png";
        var filePath = Path.Combine(_storagePath, fileName);
        await File.WriteAllBytesAsync(filePath, "Test content"u8.ToArray());
        _createdFiles.Add(fileName);

        // Act
        await using var stream = await _service.GetImageStreamAsync(fileName);

        // Assert
        var fileStream = stream.Should().BeOfType<FileStream>().Subject;
        fileStream.IsAsync.Should().BeTrue();
    }

    #endregion

    #region GetImageStreamAsync Error Tests

    [Fact(DisplayName = "GetImageStreamAsync should throw NotFoundException when file does not exist")]
    public async Task GetImageStreamAsync_WhenFileNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        const string nonExistentFileName = "non-existent.png";

        // Act
        var action = async () => await _service.GetImageStreamAsync(nonExistentFileName);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage(StorageErrors.RetrieveFailed);
    }

    [Theory(DisplayName = "GetImageStreamAsync should throw for various non-existent files")]
    [InlineData("invalid-file.png")]
    [InlineData("another-missing.jpg")]
    public async Task GetImageStreamAsync_WithNonExistentFiles_ShouldThrow(string fileName)
    {
        // Act
        var action = async () => await _service.GetImageStreamAsync(fileName);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage(StorageErrors.RetrieveFailed);
    }

    #endregion

    #region DeleteAsync Success Tests

    [Fact(DisplayName = "DeleteAsync should delete file successfully")]
    public async Task DeleteAsync_WhenFileExists_ShouldDeleteFile()
    {
        // Arrange
        const string fileName = "delete-test.png";
        var filePath = Path.Combine(_storagePath, fileName);
        await File.WriteAllBytesAsync(filePath, "Test content"u8.ToArray());

        // Verify file exists
        File.Exists(filePath).Should().BeTrue();

        // Act
        var action = async () => await _service.DeleteAsync(fileName);

        // Assert
        await action.Should().NotThrowAsync();
        File.Exists(filePath).Should().BeFalse();
    }

    #endregion

    #region DeleteAsync Error Tests

    [Fact(DisplayName = "DeleteAsync should throw NotFoundException when file does not exist")]
    public async Task DeleteAsync_WhenFileNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        const string nonExistentFileName = "non-existent.png";

        // Act
        var action = async () => await _service.DeleteAsync(nonExistentFileName);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage(StorageErrors.ImageNotFound);
    }

    #endregion

    #region Integration Tests

    [Fact(DisplayName = "Complete workflow should work end-to-end")]
    public async Task CompleteWorkflow_UploadGetDelete_ShouldWorkEndToEnd()
    {
        // Arrange
        var testContent = "Complete workflow test content"u8.ToArray();
        const string fileName = "workflow-test.jpg";

        // Act & Assert - Upload
        using (var uploadStream = new MemoryStream(testContent))
        {
            await _service.UploadAsync(uploadStream, fileName, "image/jpeg");
        }

        var filePath = Path.Combine(_storagePath, fileName);
        File.Exists(filePath).Should().BeTrue();

        // Act & Assert - Get
        await using (var retrievedStream = await _service.GetImageStreamAsync(fileName))
        {
            using var contentStream = new MemoryStream();
            await retrievedStream.CopyToAsync(contentStream);
            contentStream.ToArray().Should().BeEquivalentTo(testContent);
        }

        // Act & Assert - Delete
        await _service.DeleteAsync(fileName);
        File.Exists(filePath).Should().BeFalse();

        // Verify deletion
        var getAfterDelete = async () => await _service.GetImageStreamAsync(fileName);
        await getAfterDelete.Should().ThrowAsync<NotFoundException>();
    }

    [Fact(DisplayName = "Should handle concurrent uploads correctly")]
    public async Task ConcurrentUploads_ShouldAllSucceed()
    {
        // Arrange
        var tasks = new List<Task>();
        var fileNames = new List<string>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            var fileName = $"concurrent-{i}.png";
            fileNames.Add(fileName);
            var content = Encoding.UTF8.GetBytes($"Concurrent content {i}");

            var localFileName = fileName; // Capture for closure
            var localContent = content;
            
            var task = Task.Run(async () =>
            {
                using var stream = new MemoryStream(localContent);
                await _service.UploadAsync(stream, localFileName, "image/png");
            });
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
        _createdFiles.AddRange(fileNames);

        // Assert
        foreach (var fileName in fileNames)
        {
            var filePath = Path.Combine(_storagePath, fileName);
            File.Exists(filePath).Should().BeTrue();
        }
    }

    [Fact(DisplayName = "Should handle large file upload")]
    public async Task UploadAsync_WithLargeFile_ShouldWork()
    {
        // Arrange
        const string fileName = "large-file.png";
        var largeContent = new byte[1024 * 1024]; // 1MB
        new Random().NextBytes(largeContent);

        // Act
        using (var stream = new MemoryStream(largeContent))
        {
            await _service.UploadAsync(stream, fileName, "image/png");
        }

        _createdFiles.Add(fileName);

        // Assert
        var filePath = Path.Combine(_storagePath, fileName);
        File.Exists(filePath).Should().BeTrue();

        // Verify content
        var savedContent = await File.ReadAllBytesAsync(filePath);
        savedContent.Should().BeEquivalentTo(largeContent);
    }

    [Fact(DisplayName = "Should handle empty file upload")]
    public async Task UploadAsync_WithEmptyFile_ShouldWork()
    {
        // Arrange
        const string fileName = "empty.png";
        using var stream = new MemoryStream();

        // Act
        await _service.UploadAsync(stream, fileName, "image/png");
        _createdFiles.Add(fileName);

        // Assert
        var filePath = Path.Combine(_storagePath, fileName);
        File.Exists(filePath).Should().BeTrue();

        var fileInfo = new FileInfo(filePath);
        fileInfo.Length.Should().Be(0);
    }

    #endregion

    #region Helper Methods

    private void VerifyLogCalled(LogLevel level)
    {
        _mockLogger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    #endregion

    #region Cleanup

    public void Dispose()
    {
        foreach (var fileName in _createdFiles)
        {
            try
            {
                var filePath = Path.Combine(_storagePath, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
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