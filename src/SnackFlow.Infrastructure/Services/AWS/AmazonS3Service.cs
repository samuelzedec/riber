using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.Exceptions;
using SnackFlow.Domain.Constants;

namespace SnackFlow.Infrastructure.Services.AWS;

public sealed class AmazonS3Service : IImageStorageService
{
    private readonly IAmazonS3 _amazonS3;
    private readonly ILogger<AmazonS3Service> _logger;
    private readonly string _bucketName;

    public AmazonS3Service(
        IAmazonS3 amazonS3,
        IConfiguration configuration,
        ILogger<AmazonS3Service> logger)
    {
        _amazonS3 = amazonS3;
        _logger = logger;
        _bucketName = configuration["AWS:S3:BucketImagesName"] 
            ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null");
        
        if (string.IsNullOrWhiteSpace(_bucketName))
            throw new InvalidOperationException("AWS S3 bucket name is not configured. Please set 'AWS:S3:BucketImagesName' in configuration.");
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName, nameof(fileName));
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType, nameof(contentType));

        if (!IImageStorageService.IsValidImageType(contentType))
            throw new BadRequestException(ErrorMessage.Image.IsInvalid);

        var uniqueImageName = $"{Guid.CreateVersion7()}{Path.GetExtension(fileName).ToLowerInvariant()}";
        try
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = uniqueImageName,
                InputStream = stream,
                ContentType = contentType,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
                StorageClass = S3StorageClass.Standard
            };

            await _amazonS3.PutObjectAsync(request);

            _logger.LogDebug("Successfully uploaded image {FileName} to S3 bucket {BucketName}",
                uniqueImageName, _bucketName);

            return uniqueImageName;
        }
        catch (AmazonS3Exception s3Ex) when (s3Ex.ErrorCode == "AccessDenied")
        {
            _logger.LogError(s3Ex, "Access denied when uploading image {FileName} to S3 bucket {BucketName}",
                uniqueImageName, _bucketName);
            throw new InternalException("Storage service is temporarily unavailable. Please try again later.");
        }
        catch (AmazonS3Exception s3Ex) when (s3Ex.ErrorCode == "NoSuchBucket")
        {
            _logger.LogError(s3Ex, "S3 bucket {BucketName} not found when uploading image {FileName}",
                _bucketName, uniqueImageName);
            throw new InternalException("Storage configuration error. Please contact support.");
        }
        catch (AmazonS3Exception s3Ex)
        {
            _logger.LogError(s3Ex, "S3 error when uploading image {FileName}: {ErrorCode} - {ErrorMessage}",
                uniqueImageName, s3Ex.ErrorCode, s3Ex.Message);
            throw new InternalException("Failed to upload image. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when uploading image {FileName} to S3", uniqueImageName);
            throw new InternalException("An unexpected error occurred while uploading the image.");
        }
    }

    public async Task<Stream> GetImageStreamAsync(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName, nameof(fileName));
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName
            };
            
            var response = await _amazonS3.GetObjectAsync(request);
            
            _logger.LogDebug("Successfully retrieved image {FileName} from S3 bucket {BucketName}", 
                fileName, _bucketName);
            
            return response.ResponseStream;
        }
        catch (AmazonS3Exception s3Ex) when (s3Ex.ErrorCode == "NoSuchKey")
        {
            _logger.LogWarning("Image {FileName} not found in S3 bucket {BucketName}", fileName, _bucketName);
            throw new NotFoundException(ErrorMessage.Image.NoExists);
        }
        catch (AmazonS3Exception s3Ex) when (s3Ex.ErrorCode == "AccessDenied")
        {
            _logger.LogError(s3Ex, "Access denied when retrieving image {FileName} from S3 bucket {BucketName}", 
                fileName, _bucketName);
            throw new InternalException("Storage service is temporarily unavailable. Please try again later.");
        }
        catch (AmazonS3Exception s3Ex)
        {
            _logger.LogError(s3Ex, "S3 error when retrieving image {FileName}: {ErrorCode} - {ErrorMessage}", 
                fileName, s3Ex.ErrorCode, s3Ex.Message);
            throw new InternalException("Failed to retrieve image. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when retrieving image {FileName} from S3", fileName);
            throw new InternalException("An unexpected error occurred while retrieving the image.");
        }
    }

    public async Task DeleteAsync(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName, nameof(fileName));
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName
            };
            
            await _amazonS3.DeleteObjectAsync(request);
            
            _logger.LogDebug("Successfully deleted image {FileName} from S3 bucket {BucketName}", 
                fileName, _bucketName);
        }
        catch (AmazonS3Exception s3Ex) when (s3Ex.ErrorCode == "AccessDenied")
        {
            _logger.LogError(s3Ex, "Access denied when deleting image {FileName} from S3 bucket {BucketName}", 
                fileName, _bucketName);
            throw new InternalException("Storage service is temporarily unavailable. Please try again later.");
        }
        catch (AmazonS3Exception s3Ex)
        {
            _logger.LogError(s3Ex, "S3 error when deleting image {FileName}: {ErrorCode} - {ErrorMessage}", 
                fileName, s3Ex.ErrorCode, s3Ex.Message);
            throw new InternalException("Failed to delete image. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when deleting image {FileName} from S3", fileName);
            throw new InternalException("An unexpected error occurred while deleting the image.");
        }
    }
}