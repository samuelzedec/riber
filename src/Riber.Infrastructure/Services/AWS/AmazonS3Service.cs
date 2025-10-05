using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Services;
using Riber.Application.Exceptions;
using Riber.Domain.Constants.Messages.Common;

namespace Riber.Infrastructure.Services.AWS;

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
            throw new InvalidOperationException(
                "AWS S3 bucket name is not configured. Please set 'AWS:S3:BucketImagesName' in configuration.");
    }

    public async Task UploadAsync(Stream stream, string fileName, string contentType)
    {
        try
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                InputStream = stream,
                ContentType = contentType,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
                StorageClass = S3StorageClass.Standard
            };

            await _amazonS3.PutObjectAsync(request);
            _logger.LogDebug("Successfully uploaded image {FileName} to S3 bucket {BucketName}",
                fileName, _bucketName);
        }
        catch (AmazonS3Exception s3Ex) when (s3Ex.ErrorCode == "AccessDenied")
        {
            _logger.LogError(UnexpectedErrors.ForLogging(nameof(AmazonS3Service), s3Ex));
            throw new InternalException(StorageErrors.AccessDenied);
        }
        catch (AmazonS3Exception s3Ex) when (s3Ex.ErrorCode == "NoSuchBucket")
        {
            _logger.LogError(UnexpectedErrors.ForLogging(nameof(AmazonS3Service), s3Ex));
            throw new InternalException(StorageErrors.BucketNotFound);
        }
        catch (AmazonS3Exception s3Ex)
        {
            _logger.LogError(UnexpectedErrors.ForLogging(nameof(AmazonS3Service), s3Ex));
            throw new InternalException(StorageErrors.UploadFailed);
        }
        catch (Exception ex)
        {
            _logger.LogError(UnexpectedErrors.ForLogging(nameof(AmazonS3Service), ex));
            throw new InternalException(UnexpectedErrors.Response);
        }
    }

    public async Task<Stream> GetImageStreamAsync(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName, nameof(fileName));
        try
        {
            var request = new GetObjectRequest { BucketName = _bucketName, Key = fileName };
            var response = await _amazonS3.GetObjectAsync(request);

            _logger.LogDebug("Successfully retrieved image {FileName} from S3 bucket {BucketName}", fileName,
                _bucketName);

            return response.ResponseStream;
        }
        catch (AmazonS3Exception s3Ex) when (s3Ex.ErrorCode == "NoSuchKey")
        {
            _logger.LogError(UnexpectedErrors.ForLogging(nameof(AmazonS3Service), s3Ex));
            throw new NotFoundException(StorageErrors.ImageNotFound);
        }
        catch (AmazonS3Exception s3Ex) when (s3Ex.ErrorCode == "AccessDenied")
        {
            _logger.LogError(UnexpectedErrors.ForLogging(nameof(AmazonS3Service), s3Ex));
            throw new InternalException(StorageErrors.RetrieveAccessDenied);
        }
        catch (AmazonS3Exception s3Ex)
        {
            _logger.LogError(UnexpectedErrors.ForLogging(nameof(AmazonS3Service), s3Ex));
            throw new InternalException(StorageErrors.RetrieveFailed);
        }
        catch (Exception ex)
        {
            _logger.LogError(UnexpectedErrors.ForLogging(nameof(AmazonS3Service), ex));
            throw new InternalException(UnexpectedErrors.Response);
        }
    }

    public async Task DeleteAsync(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName, nameof(fileName));
        try
        {
            var request = new DeleteObjectRequest { BucketName = _bucketName, Key = fileName };

            await _amazonS3.DeleteObjectAsync(request);
            _logger.LogDebug("Successfully deleted image {FileName} from S3 bucket {BucketName}",
                fileName, _bucketName);
        }
        catch (Exception ex)
        {
            _logger.LogError(UnexpectedErrors.ForLogging(nameof(AmazonS3Service), ex));
            throw new InternalException(UnexpectedErrors.Response);
        }
    }
}