using System.Security.Cryptography.X509Certificates;
using SnackFlow.Application.Abstractions.Services;

namespace SnackFlow.Infrastructure.Services.Local;

public sealed class LocalCertificateService : ICertificateService
{
    private const X509KeyStorageFlags TagsForCertificates =
        X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet;

    public X509Certificate2 LoadCertificate(string key, string password)
        => X509CertificateLoader.LoadPkcs12FromFile(
            key,
            password,
            TagsForCertificates
        );
}