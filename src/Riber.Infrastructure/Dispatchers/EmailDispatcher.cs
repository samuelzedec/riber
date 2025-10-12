using Newtonsoft.Json;
using Quartz;
using Riber.Application.Abstractions.Dispatchers;
using Riber.Application.Extensions;
using Riber.Domain.Enums;
using Riber.Infrastructure.BackgroundJobs;

namespace Riber.Infrastructure.Dispatchers;

public sealed class EmailDispatcher(ISchedulerFactory schedulerFactory)
    : IEmailDispatcher
{
    public async Task SendAsync(
        EmailAddress emailAddress,
        object emailData,
        CancellationToken cancellationToken = default)
    {
        var dataInString = JsonConvert.SerializeObject(emailData);
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