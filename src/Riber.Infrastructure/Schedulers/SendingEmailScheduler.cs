using Quartz;
using Riber.Infrastructure.BackgroundJobs;

namespace Riber.Infrastructure.Schedulers;

public static class SendingEmailScheduler
{
    public static void Configure(IServiceCollectionQuartzConfigurator quartzConfigurator)
    {
        var jobKey = new JobKey(nameof(SendingEmailJob));
        quartzConfigurator.AddJob<SendingEmailJob>(options => options
            .WithIdentity(jobKey)
            .WithDescription("Envia de e-mails para notificar o usuário")
            .StoreDurably());
    }
}