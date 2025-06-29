using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace SnackFlow.Infrastructure.Settings;

public sealed class AccessTokenSettings
{
    [Required] public string Source { get; set; } = string.Empty;
    [Required] public string Secret { get; set; } = string.Empty;
    [Required] public string Key { get; set; } = string.Empty;
    [Required] public string Issuer { get; set; } = string.Empty;
    [Required] public string Audience { get; set; } = string.Empty;
    [Required] [Range(10, 20)] public int ExpirationInMinutes { get; set; }

    public X509Certificate2 GenerateCertificate()
        => X509CertificateLoader.LoadPkcs12FromFile(
            Source, Secret, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
}