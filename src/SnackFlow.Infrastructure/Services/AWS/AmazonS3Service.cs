using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.Exceptions;
using SnackFlow.Domain.Constants;

namespace SnackFlow.Infrastructure.Services.AWS;

public sealed class AmazonS3Service(
    IAmazonS3 amazonS3,
    IConfiguration configuration,
    ILogger<AmazonS3Service> logger)
    : IImageStorageService
{
    private readonly string _bucketName = configuration["AWS:S3:BucketImagesName"] ?? string.Empty;

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType)
    {
        if (string.IsNullOrWhiteSpace(_bucketName))
            throw new InternalException(ErrorMessage.Exception.Unexpected("AmazonS3Service", "Bucket name is not configured."));

        if (!IImageStorageService.IsValidImageType(contentType))
            throw new BadRequestException(ErrorMessage.Image.IsInvalid);

        try
        {
            var uniqueImageName = $"{Guid.CreateVersion7()}{Path.GetExtension(fileName).ToLowerInvariant()}";
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = uniqueImageName,
                InputStream = stream,
                ContentType = contentType,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
                StorageClass = S3StorageClass.Standard
            };
            
            await amazonS3.PutObjectAsync(request);
            return uniqueImageName;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessage.Exception.Unexpected(ex.GetType().Name, ex.Message));
            throw;
        }
    }

    public async Task<Stream> GetImageStreamAsync(string fileName)
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName
            };
            
            var response = await amazonS3.GetObjectAsync(request);
            return response.ResponseStream;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessage.Exception.Unexpected(ex.GetType().Name, ex.Message));
            throw;
        }
    }

    public Task DeleteAsync(string fileName)
    {
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName
            };
            
            return amazonS3.DeleteObjectAsync(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessage.Exception.Unexpected(ex.GetType().Name, ex.Message));
            throw;
        }
    }
}