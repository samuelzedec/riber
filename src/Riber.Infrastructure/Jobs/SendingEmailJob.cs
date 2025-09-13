using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Riber.Application.Abstractions.Services.Email;
using Riber.Domain.Constants;

namespace Riber.Infrastructure.Jobs;

public sealed class SendingEmailJob(
    IEmailService emailService,
    IEmailTemplateRender templateEmailRender,
    IEmailConcurrencyService emailConcurrencyService,
    ILogger<SendingEmailJob> logger) 
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        using var semaphoreRelease = await emailConcurrencyService.AcquireAsync();
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
            logger.LogError(ex, ErrorMessage.Exception.Unexpected(ex.GetType().Name, ex.Message));
            throw;
        }
    }
}