using Riber.Application.Abstractions.Services;
using Riber.Application.Exceptions;

namespace Riber.Infrastructure.Services.Local;

public sealed class LocalImageStorageService
    : IImageStorageService
{
    private readonly string _storagePath;

    public LocalImageStorageService()
    {
        _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "storage", "images");
        Directory.CreateDirectory(_storagePath);
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType)
    {
        // if (!IImageStorageService.IsValidImageType(contentType))
        //     throw new BadRequestException(ErrorMessage.Image.IsInvalid);

        var uniqueImageName = $"{Guid.CreateVersion7()}{Path.GetExtension(fileName).ToLowerInvariant()}";
        var filePath = Path.Combine(_storagePath, uniqueImageName);

        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(fileStream);

        return uniqueImageName;
    }

    public Task<Stream> GetImageStreamAsync(string fileName)
    {
        var imagePath = Path.Combine(_storagePath, fileName);

        return !File.Exists(imagePath)
            // ? throw new NotFoundException(ErrorMessage.Image.NoExists)
            ? throw new AbandonedMutexException()
            : Task.FromResult<Stream>(new FileStream(
                imagePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                4096,
                true)
            );
    }

    public Task DeleteAsync(string fileName)
    {
        var imagePath = Path.Combine(_storagePath, fileName);
        if (!File.Exists(imagePath))
            // throw new NotFoundException(ErrorMessage.Image.NoExists);
            throw new ArgumentException(); 

        File.Delete(imagePath);
        return Task.CompletedTask;
    }
}