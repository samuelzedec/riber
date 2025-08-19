using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.Abstractions.Services.Concurrency;
using SnackFlow.Domain.Constants;

namespace SnackFlow.Infrastructure.Jobs;

public sealed class SendingEmailJob(
    IEmailService emailService,
    IEmailTemplateService templateEmailService,
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
            var emailAddress = jobDataMap.GetString("EmailAddress")!;
            var data = JsonConvert.DeserializeObject<JObject>(jobDataMap.GetString("DataEmail")!)!;
            var to = data["to"]?.ToString()!;
            var subject = data["subject"]?.ToString()!;
            var templateProcessed = await templateEmailService.GetTemplateAsync(data);
            
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