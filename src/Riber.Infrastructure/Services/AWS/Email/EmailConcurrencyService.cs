using Microsoft.Extensions.Configuration;
using Riber.Application.Abstractions.Services.Email;

namespace Riber.Infrastructure.Services.AWS.Email;

public sealed class EmailConcurrencyService
    : IEmailConcurrencyService
{
    private readonly SemaphoreSlim _semaphoreSlim;

    public EmailConcurrencyService(IConfiguration configuration)
    {
        var maxConcurrent = configuration.GetValue("AWS:SES:MaxConcurrentEmails", 3);
        _semaphoreSlim = new SemaphoreSlim(maxConcurrent, maxConcurrent);
    }
    
    public async Task<IDisposable> AcquireAsync(CancellationToken cancellationToken = default)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);
        return new SemaphoreReleaser(_semaphoreSlim);
    }

    private sealed class SemaphoreReleaser(SemaphoreSlim semaphoreSlim) : IDisposable
    {
        public void Dispose() => semaphoreSlim.Release();
    }
}