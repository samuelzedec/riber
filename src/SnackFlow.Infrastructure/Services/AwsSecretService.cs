using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SnackFlow.Infrastructure.Services.Abstractions;

namespace SnackFlow.Infrastructure.Services;

public class AwsSecretService(
    IMemoryCache cache,
    ILogger<AwsSecretService> logger) 
    : ISecretService
{
    public Task<byte[]> GetSecretAsync(string key, string password)
    {
        try
        {
            var cacheKey = $"certificate_secret_{key}";
            if (cache.TryGetValue(cacheKey, out byte[]? value) && value is not null)
                return Task.FromResult(value);

            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex.StackTrace);
            throw;
        }
    }

    private void CreateSecretCache(string source, byte[] certificate)
    {
        var cacheKey = $"certificate_secret_{source}";
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(4),
            SlidingExpiration = TimeSpan.FromMinutes(30),
            Priority = CacheItemPriority.High
        };
        cache.Set(cacheKey, certificate, cacheEntryOptions);
    }
}