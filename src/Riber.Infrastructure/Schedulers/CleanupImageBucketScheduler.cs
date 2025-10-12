using Quartz;
using Riber.Infrastructure.BackgroundJobs;

namespace Riber.Infrastructure.Schedulers;

internal static class CleanupImageBucketScheduler
{
    internal static void Configure(IServiceCollectionQuartzConfigurator quartzConfigurator)
    {
        var jobKey = new JobKey(nameof(CleanupImageBucketJob));
        quartzConfigurator.AddJob<CleanupImageBucketJob>(options => options
            .WithIdentity(jobKey)
            .WithDescription("Faz a limpeza de imagens não utilizadas mais da bucket"));

        quartzConfigurator.AddTrigger(options => options
            .WithIdentity($"{jobKey}-trigger")
            .WithCronSchedule("0 0 3 ? * SUN *")
            .WithDescription("Dispara o job de limpeza de imagens na bucket toda semana as 3:00 da manhã no domingo")
            .ForJob(jobKey));
    }
}