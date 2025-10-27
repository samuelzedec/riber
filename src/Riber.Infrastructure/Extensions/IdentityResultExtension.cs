using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Riber.Infrastructure.Extensions;

public static class IdentityResultExtension
{
    public static void LogIdentityErrors<T>(
        this IdentityResult identityResult, 
        string message, 
        ILogger<T> logger)
    {
        var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
        logger.LogError("{Message}: {Errors}", message, errors);
    }
}