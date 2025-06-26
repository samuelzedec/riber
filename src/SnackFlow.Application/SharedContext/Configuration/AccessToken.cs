using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace SnackFlow.Application.SharedContext.Configuration;
public sealed class AccessToken
{
    [Required] public string Source { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    [Required] public string Key { get; set; } = string.Empty;
    [Required] public string Issuer { get; set; } = string.Empty;
    [Required] public string Audience { get; set; } = string.Empty;
    [Required] [Range(10, 20)] public int ExpirationInMinutes { get; set; }

    public X509Certificate2 GenerateCertificate()
    {
        try
        {
            if (File.Exists(Source))
                X509CertificateLoader.LoadPkcs12FromFile(Source, Secret, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
            
            var certBytes = Convert.FromBase64String(Source);
            return X509CertificateLoader.LoadPkcs12(
                certBytes, Secret,
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load certificate from source: {Source}", ex);
        }
    }
}