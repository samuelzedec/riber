using Newtonsoft.Json;
using Quartz;
using Riber.Application.Abstractions.Schedulers;
using Riber.Application.Extensions;
using Riber.Domain.Enums;
using Riber.Infrastructure.Jobs;

namespace Riber.Infrastructure.Schedulers;

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