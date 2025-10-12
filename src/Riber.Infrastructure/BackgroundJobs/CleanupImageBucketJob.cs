using Microsoft.Extensions.Logging;
using Quartz;
using Riber.Application.Abstractions.Services;
using Riber.Domain.Entities;
using Riber.Domain.Repositories;

namespace Riber.Infrastructure.BackgroundJobs;

internal sealed class CleanupImageBucketJob(
    IImageStorageService imageStorageService,
    IProductRepository productRepository,
    ILogger<CleanupImageBucketJob> logger)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var unusedImages = await productRepository.GetUnusedImagesAsync(context.CancellationToken);
        if (unusedImages.Count == 0)
        {
            logger.LogInformation("No unused images found for cleanup.");
            return;
        }

        logger.LogInformation("Starting cleanup of {Count} unused images.", unusedImages.Count);
        int deletedCount = 0;
        int failedCount = 0;

        foreach (Image unusedImage in unusedImages)
        {
            try
            {
                await imageStorageService.DeleteAsync(unusedImage);
                deletedCount++;
                logger.LogDebug("Successfully deleted image {ImageId}.", unusedImage.Id);
            }
            catch (Exception ex)
            {
                failedCount++;
                logger.LogError(ex,
                    "Failed to delete image {ImageId} - {ImageName}.",
                    unusedImage.Id,
                    unusedImage.ToString()
                );
            }
        }

        logger.LogInformation(
            "Image cleanup completed. Successfully deleted: {Deleted}, Failed: {Failed}",
            deletedCount,
            failedCount
        );
    }
}