using System.Security.Cryptography.X509Certificates;

namespace Riber.Application.Abstractions.Services;

/// <summary>
/// Define um serviço para carregamento de certificados X.509.
/// </summary>
public interface ICertificateService
{
    /// <summary>
    /// Carrega um certificado X.509 de um arquivo especificado usando a chave e senha fornecidas.
    /// </summary>
    /// <param name="key">O caminho do arquivo do certificado X.509.</param>
    /// <param name="password">A senha utilizada para acessar o certificado.</param>
    /// <returns>Uma instância de <see cref="X509Certificate2"/> representando o certificado X.509 carregado.</returns>
    X509Certificate2 LoadCertificate(string key, string password);
}