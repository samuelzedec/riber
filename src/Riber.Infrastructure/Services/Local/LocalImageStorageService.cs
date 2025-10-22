using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Services;
using Riber.Application.Exceptions;
using Riber.Domain.Constants.Messages.Common;

namespace Riber.Infrastructure.Services.Local;

public sealed class LocalImageStorageService
    : IImageStorageService
{
    private readonly string _storagePath;
    private readonly ILogger<LocalImageStorageService> _logger;

    public LocalImageStorageService(ILogger<LocalImageStorageService> logger)
    {
        _logger = logger;
        _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "storage", "images");
        Directory.CreateDirectory(_storagePath);
    }

    public async Task UploadAsync(Stream stream, string fileName, string contentType)
    {
        try
        {
            var filePath = Path.Combine(_storagePath, fileName);
            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await stream.CopyToAsync(fileStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "[{ClassName}] exceção inesperada: {ExceptionType} - {ExceptionMessage}",
                nameof(LocalImageStorageService),
                ex.GetType(),
                ex.Message);
            throw new InternalException(StorageErrors.UploadFailed);
        }
    }

    public Task<Stream> GetImageStreamAsync(string fileName)
    {
        var imagePath = Path.Combine(_storagePath, fileName);

        return !File.Exists(imagePath)
            ? throw new NotFoundException(StorageErrors.RetrieveFailed)
            : Task.FromResult<Stream>(new FileStream(
                imagePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                4096,
                true)
            );
    }

    public Task<IEnumerable<string>> DeleteAllAsync(List<string> fileKeys)
    {
        List<string> deletedKeys = [];
        foreach (var fileKey in fileKeys)
        {
            var imagePath = Path.Combine(_storagePath, fileKey);
            if (!File.Exists(imagePath))
                continue;

            File.Delete(imagePath);
            deletedKeys.Add(fileKey);
        }
        
        _logger.LogInformation("Deleted {Count} file(s)", deletedKeys.Count);
        return Task.FromResult(deletedKeys.AsEnumerable());
    }
}