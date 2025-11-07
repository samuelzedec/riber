using Microsoft.Extensions.Logging;
using Quartz;
using Riber.Application.Abstractions.Services;
using Riber.Domain.Entities.Catalog;
using Riber.Domain.Repositories;

namespace Riber.Infrastructure.BackgroundJobs;

internal sealed class CleanupImageBucketJob(
    IImageStorageService imageStorageService,
    IUnitOfWork unitOfWork,
    ILogger<CleanupImageBucketJob> logger)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var unusedImages = await unitOfWork.Products.GetUnusedImagesAsync(context.CancellationToken);
            if (unusedImages.Count == 0)
            {
                logger.LogInformation("No unused images found for cleanup.");
                return;
            }

            logger.LogInformation("Starting cleanup of {Count} unused images.", unusedImages.Count);
            var unusedImageKeys = unusedImages.Select(x => x.ToString()).ToList();

            var deletedKeys = (await imageStorageService.DeleteAllAsync(unusedImageKeys)).ToHashSet();
            await MarkImageAsDeletedAsync(unusedImages, deletedKeys);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete images from storage service.");
        }
    }

    private async Task MarkImageAsDeletedAsync(IReadOnlyList<Image> unusedImage, HashSet<string> deletedKeys)
    {
        var imagesForDeletion = unusedImage
            .Where(image => deletedKeys.Contains(image.ToString()))
            .ToList();

        if (imagesForDeletion.Count == 0)
            return;

        foreach (var image in imagesForDeletion)
            unitOfWork.Products.DeleteImage(image);

        await unitOfWork.SaveChangesAsync();
    }
}