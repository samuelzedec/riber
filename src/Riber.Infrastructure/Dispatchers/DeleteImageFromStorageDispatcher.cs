using Quartz;
using Riber.Application.Abstractions.Dispatchers;
using Riber.Infrastructure.BackgroundJobs;

namespace Riber.Infrastructure.Dispatchers;

public sealed class DeleteImageFromStorageDispatcher(ISchedulerFactory schedulerFactory)
    : IDeleteImageFromStorageDispatcher
{
    public async Task SendAsync(string imageKey, CancellationToken cancellationToken = default)
    {
        var jobData = new JobDataMap { ["imageKey"] = imageKey };

        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.TriggerJob(
            new JobKey(nameof(DeleteImageFromStorageJob)),
            jobData,
            cancellationToken
        );
    }
}