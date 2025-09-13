using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace Riber.Infrastructure.Settings;

public sealed class AccessTokenSettings
{
    [Required] public string Key { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    [Required] public string Issuer { get; set; } = string.Empty;
    [Required] public string Audience { get; set; } = string.Empty;
    [Required] [Range(10, 20)] public int ExpirationInMinutes { get; set; }
}