using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Riber.Application.Abstractions.Services.Email;

namespace Riber.Infrastructure.BackgroundJobs;

internal sealed class SendingEmailJob(
    IEmailService emailService,
    IEmailTemplateRender templateEmailRender,
    IEmailConcurrencyService emailConcurrencyService,
    ILogger<SendingEmailJob> logger)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        using var _ = await emailConcurrencyService.AcquireAsync();
        var jobDataMap = context.Trigger.JobDataMap;
        try
        {
            var emailAddress = jobDataMap.GetString("emailAddress")!;
            var emailPayload = JsonConvert.DeserializeObject<JObject>(jobDataMap.GetString("emailPayload")!)!;
            var to = emailPayload["to"]?.ToString()!;
            var subject = emailPayload["subject"]?.ToString()!;
            var templateProcessed = await templateEmailRender.GetTemplateAsync(emailPayload);

            await emailService.SendAsync(
                to,
                subject,
                templateProcessed,
                emailAddress
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Permanent failure sending email to {EmailAddress}. Will NOT retry.",
                jobDataMap.GetString("emailAddress")
            );
        }
    }
}