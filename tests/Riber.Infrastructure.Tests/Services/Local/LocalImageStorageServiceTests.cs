using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Riber.Application.Exceptions;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Tests;
using Riber.Infrastructure.Services.Local;

namespace Riber.Infrastructure.Tests.Services.Local;

public sealed class LocalImageStorageServiceTests : BaseTest, IDisposable
{
    #region Setup

    private readonly Mock<ILogger<LocalImageStorageService>> _mockLogger;
    private readonly LocalImageStorageService _service;
    private readonly string _storagePath;
    private readonly List<string> _createdFiles = [];

    public LocalImageStorageServiceTests()
    {
        _mockLogger = new Mock<ILogger<LocalImageStorageService>>();
        _service = new LocalImageStorageService(_mockLogger.Object);
        _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "storage", "images");
    }

    #endregion

    #region UploadAsync Success Tests

    [Trait("Category", "Unit")]
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

    [Trait("Category", "Unit")]
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

    [Trait("Category", "Unit")]
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

    [Trait("Category", "Unit")]
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

    [Trait("Category", "Unit")]
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

    [Trait("Category", "Unit")]
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

    [Trait("Category", "Unit")]
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

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "DeleteAllAsync should delete multiple files successfully")]
    public async Task DeleteAllAsync_WhenFilesExist_ShouldDeleteAllFiles()
    {
        // Arrange
        var fileNames = new List<string> { "delete-1.png", "delete-2.jpg", "delete-3.webp" };
        var filePaths = new List<string>();

        foreach (var fileName in fileNames)
        {
            var filePath = Path.Combine(_storagePath, fileName);
            await File.WriteAllBytesAsync(filePath, "Test content"u8.ToArray());
            filePaths.Add(filePath);
            _createdFiles.Add(fileName);
        }

        // Verify files exist
        foreach (var filePath in filePaths)
        {
            File.Exists(filePath).Should().BeTrue();
        }

        // Act
        var result = (await _service.DeleteAllAsync(fileNames)).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(fileNames);

        foreach (var filePath in filePaths)
        {
            File.Exists(filePath).Should().BeFalse();
        }
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "DeleteAllAsync should delete single file successfully")]
    public async Task DeleteAllAsync_WithSingleFile_ShouldDeleteFile()
    {
        // Arrange
        const string fileName = "single-delete.png";
        var filePath = Path.Combine(_storagePath, fileName);
        await File.WriteAllBytesAsync(filePath, "Test content"u8.ToArray());
        _createdFiles.Add(fileName);

        // Act
        var result = (await _service.DeleteAllAsync([fileName])).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(fileName);
        File.Exists(filePath).Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "DeleteAllAsync should skip non-existent files")]
    public async Task DeleteAllAsync_WithMixedExistingAndNonExisting_ShouldReturnOnlyDeleted()
    {
        // Arrange
        var existingFile = "existing.png";
        var nonExistingFile = "non-existing.png";
        var anotherExisting = "another-existing.jpg";

        var filePath1 = Path.Combine(_storagePath, existingFile);
        var filePath3 = Path.Combine(_storagePath, anotherExisting);

        await File.WriteAllBytesAsync(filePath1, "Test content"u8.ToArray());
        await File.WriteAllBytesAsync(filePath3, "Test content"u8.ToArray());

        _createdFiles.Add(existingFile);
        _createdFiles.Add(anotherExisting);

        var fileNames = new List<string> { existingFile, nonExistingFile, anotherExisting };

        // Act
        var result = (await _service.DeleteAllAsync(fileNames)).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(existingFile);
        result.Should().Contain(anotherExisting);
        result.Should().NotContain(nonExistingFile);

        File.Exists(filePath1).Should().BeFalse();
        File.Exists(filePath3).Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "DeleteAllAsync should return empty list when no files exist")]
    public async Task DeleteAllAsync_WhenNoFilesExist_ShouldReturnEmptyList()
    {
        // Arrange
        var fileNames = new List<string> { "non-existent-1.png", "non-existent-2.jpg" };

        // Act
        var result = await _service.DeleteAllAsync(fileNames);

        // Assert
        result.Should().BeEmpty();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "DeleteAllAsync should handle empty list")]
    public async Task DeleteAllAsync_WithEmptyList_ShouldReturnEmptyList()
    {
        // Act
        var result = await _service.DeleteAllAsync([]);

        // Assert
        result.Should().BeEmpty();
    }

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "DeleteAllAsync should delete files with different extensions")]
    [InlineData("file1.jpg", "file2.png", "file3.webp")]
    [InlineData("doc.pdf", "image.gif", "photo.jpeg")]
    public async Task DeleteAllAsync_WithDifferentExtensions_ShouldDeleteAll(params string[] fileNames)
    {
        // Arrange
        var filePaths = new List<string>();

        foreach (var fileName in fileNames)
        {
            var filePath = Path.Combine(_storagePath, fileName);
            await File.WriteAllBytesAsync(filePath, "Test content"u8.ToArray());
            filePaths.Add(filePath);
            _createdFiles.Add(fileName);
        }

        // Act
        var result = (await _service.DeleteAllAsync([.. fileNames])).ToList();

        // Assert
        result.Should().HaveCount(fileNames.Length);
        result.Should().BeEquivalentTo(fileNames);

        foreach (var filePath in filePaths)
        {
            File.Exists(filePath).Should().BeFalse();
        }
    }

    #endregion

    #region DeleteAllAsync Integration Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "DeleteAllAsync should handle concurrent deletions")]
    public async Task DeleteAllAsync_ConcurrentDeletions_ShouldWork()
    {
        // Arrange
        var tasks = new List<Task<IEnumerable<string>>>();
        var allFileNames = new List<string>();

        for (int batch = 0; batch < 3; batch++)
        {
            var batchFiles = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                var fileName = $"concurrent-delete-batch{batch}-{i}.png";
                var filePath = Path.Combine(_storagePath, fileName);
                await File.WriteAllBytesAsync(filePath, "Test content"u8.ToArray());
                batchFiles.Add(fileName);
                allFileNames.Add(fileName);
                _createdFiles.Add(fileName);
            }

            var localBatch = batchFiles;
            tasks.Add(_service.DeleteAllAsync(localBatch));
        }

        // Act
        var results = await Task.WhenAll(tasks);

        // Assert
        var totalDeleted = results.SelectMany(r => r).Count();
        totalDeleted.Should().Be(15);

        foreach (var filePath in allFileNames.Select(fileName => Path.Combine(_storagePath, fileName)))
            File.Exists(filePath).Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "DeleteAllAsync should log deletion summary")]
    public async Task DeleteAllAsync_ShouldLogDeletionSummary()
    {
        // Arrange
        var fileNames = new List<string> { "log-test-1.png", "log-test-2.jpg" };

        foreach (var fileName in fileNames)
        {
            var filePath = Path.Combine(_storagePath, fileName);
            await File.WriteAllBytesAsync(filePath, "Test content"u8.ToArray());
            _createdFiles.Add(fileName);
        }

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

    #region UploadAsync Error Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "UploadAsync should throw InternalException when stream is disposed")]
    public async Task UploadAsync_WhenStreamDisposed_ShouldThrowInternalException()
    {
        // Arrange
        var stream = new MemoryStream("Test"u8.ToArray());
        await stream.DisposeAsync(); // Stream já fechado
        const string fileName = "test.png";

        // Act
        var action = async () => await _service.UploadAsync(stream, fileName, "image/png");

        // Assert
        await action.Should().ThrowAsync<InternalException>()
            .WithMessage(StorageErrors.UploadFailed);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "UploadAsync should log error when exception occurs")]
    public async Task UploadAsync_WhenExceptionOccurs_ShouldLogError()
    {
        // Arrange
        var stream = new MemoryStream("Test"u8.ToArray());
        await stream.DisposeAsync();
        const string fileName = "test.png";

        // Act
        try
        {
            await _service.UploadAsync(stream, fileName, "image/png");
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

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "UploadAsync should throw when path is invalid")]
    public async Task UploadAsync_WhenInvalidFileName_ShouldThrowInternalException()
    {
        // Arrange
        using var stream = new MemoryStream("Test"u8.ToArray());
        const string invalidFileName = "test/\\:*?\"<>|.png"; // Caracteres inválidos

        // Act
        var action = async () => await _service.UploadAsync(stream, invalidFileName, "image/png");

        // Assert
        await action.Should().ThrowAsync<InternalException>()
            .WithMessage(StorageErrors.UploadFailed);
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