using System.Security.Cryptography.X509Certificates;
using SnackFlow.Infrastructure.Services.Abstractions;

namespace SnackFlow.Infrastructure.Services;

public sealed class CertificateService : ICertificateService
{
    private const X509KeyStorageFlags TagsForCertificates =
        X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet;

    public X509Certificate2 LoadCertificateAsync(string key, string password)
        => X509CertificateLoader.LoadPkcs12FromFile(
            key,
            password,
            TagsForCertificates
        );
}