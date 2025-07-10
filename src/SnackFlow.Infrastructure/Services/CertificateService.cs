using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SnackFlow.Infrastructure.Services.Abstractions;

namespace SnackFlow.Infrastructure.Services;

public sealed class CertificateService(
    ISecretService secretService,
    IWebHostEnvironment environment) 
    : ICertificateService
{
    private const X509KeyStorageFlags TagsForCertificates = 
        X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet;

    public async Task<X509Certificate2> LoadCertificateAsync(string key, string password)
    {
        if (!environment.IsProduction())
        {
            return X509CertificateLoader.LoadPkcs12FromFile(
                key,
                password,
                TagsForCertificates
            );
        }

        var certificateBytes = await secretService.GetSecretAsync(key, password);
        return X509CertificateLoader.LoadPkcs12(
            certificateBytes, 
            password, 
            TagsForCertificates
        );
    }
}