using System.Security.Cryptography.X509Certificates;

namespace SnackFlow.Infrastructure.Services.Abstractions;

/// <summary>
/// Define um serviço para carregamento de certificados X.509.
/// </summary>
public interface ICertificateService
{
    X509Certificate2 LoadCertificateAsync(string key, string password);
}