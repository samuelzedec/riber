using System.ComponentModel.DataAnnotations;

namespace Riber.Infrastructure.Settings;

public sealed class RefreshTokenSettings
{
    [Required] public string Key { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    [Required] public string Issuer { get; set; } = string.Empty;
    [Required] public string Audience { get; set; } = string.Empty;
    [Required] [Range(2, 15)] public int ExpirationInDays { get; set; }
}