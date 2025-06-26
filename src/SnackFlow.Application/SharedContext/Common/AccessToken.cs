using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace SnackFlow.Application.SharedContext.Common;
public sealed record AccessToken(
    [Required] string Path,
    [Required] string Password,
    [Required] string Key,
    [Required] string Issuer,
    [Required] string Audience,
    [Required] [Range(10, 20)] int ExpirationInMinutes)
{
    public X509Certificate2 GenerateCertificate()
        => X509CertificateLoader.LoadPkcs12FromFile(
            Path,
            Password,
            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
}