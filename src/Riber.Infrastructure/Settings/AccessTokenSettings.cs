using System.ComponentModel.DataAnnotations;

namespace Riber.Infrastructure.Settings;

public sealed class AccessTokenSettings
{
    [Required] public string SecretKey { get; set; } = string.Empty;
    [Required] public string Issuer { get; set; } = string.Empty;
    [Required] public string Audience { get; set; } = string.Empty;
    [Required] [Range(10, 20)] public int ExpirationInMinutes { get; set; }
}