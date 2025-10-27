using Microsoft.Extensions.Logging;
using Quartz;
using Riber.Application.Abstractions.Services;

namespace Riber.Infrastructure.BackgroundJobs;

public sealed class DeleteImageFromStorageJob(
    IImageStorageService imageStorageService,
    ILogger<DeleteImageFromStorageJob> logger)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        string imageKey = string.Empty;
        try
        {
            var jobDataMap = context.Trigger.JobDataMap;
            imageKey = jobDataMap.GetString("imageKey")!;

            logger.LogInformation("Starting deletion of image {ImageKey} from storage", imageKey);

            await imageStorageService.DeleteAllAsync(imageKey);
            logger.LogInformation("Successfully deleted image {ImageKey} from storage", imageKey);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete image {ImageKey} from storage. Job will be retried", imageKey);
        }
    }
}