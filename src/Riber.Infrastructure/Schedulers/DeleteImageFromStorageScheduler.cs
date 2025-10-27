using Quartz;
using Riber.Infrastructure.BackgroundJobs;

namespace Riber.Infrastructure.Schedulers;

public static class DeleteImageFromStorageScheduler
{
    public static void Configure(IServiceCollectionQuartzConfigurator quartzConfigurator)
    {
        var jobKey = new JobKey(nameof(DeleteImageFromStorageJob));
        quartzConfigurator.AddJob<DeleteImageFromStorageJob>(options => options
            .WithIdentity(jobKey)
            .WithDescription("Remove imagens da bucket quando é salvo na bucket e da errado no banco de dados.")
            .StoreDurably());
    }
}