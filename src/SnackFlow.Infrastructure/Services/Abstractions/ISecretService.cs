namespace SnackFlow.Infrastructure.Services.Abstractions;

/// <summary>
/// Define um serviço para recuperar segredos.
/// </summary>
public interface ISecretService
{
    /// <summary>
    /// Recupera um segredo com base na chave e senha fornecidas.
    /// </summary>
    /// <param name="key">A chave usada para identificar o segredo.</param>
    /// <param name="passqord">A senha necessária para acessar o segredo.</param>
    /// <returns>Um array de bytes representando o segredo recuperado.</returns>
    Task<byte[]> GetSecretAsync(string key, string passqord);
}