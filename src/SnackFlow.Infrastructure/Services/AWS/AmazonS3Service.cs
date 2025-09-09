using SnackFlow.Application.Abstractions.Services;

namespace SnackFlow.Infrastructure.Services.AWS;

public sealed class AmazonS3Service : IImageStorageService
{
    public Task<string> UploadAsync(Stream stream, string fileName, string contentType)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> GetImageStreamAsync(string fileName)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string fileName)
    {
        throw new NotImplementedException();
    }
}