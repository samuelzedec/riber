using Newtonsoft.Json;
using Quartz;
using SnackFlow.Application.Abstractions.Schedulers;
using SnackFlow.Application.Extensions;
using SnackFlow.Domain.Enums;
using SnackFlow.Infrastructure.Jobs;

namespace SnackFlow.Infrastructure.Schedulers;

public sealed class QuartzEmailScheduler(
    ISchedulerFactory schedulerFactory)
    : IEmailScheduler
{
    public async Task ScheduleEmailAsync(
        EmailAddress emailAddress,
        object dataEmail,
        CancellationToken cancellationToken = default)
    {
        var dataInString = JsonConvert.SerializeObject(dataEmail);
        var jobData = new JobDataMap
        {
            ["emailAddress"] = emailAddress.GetDescription(),
            ["emailPayload"] = dataInString
        };

        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.TriggerJob(
            new JobKey(nameof(SendingEmailJob)),
            jobData,
            cancellationToken
        );
    }
}