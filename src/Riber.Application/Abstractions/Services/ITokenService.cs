using Riber.Application.DTOs;

namespace Riber.Application.Abstractions.Services;

/// <summary>
/// Interface de serviço responsável pela geração de tokens de autenticação.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Gera um token para o usuário especificado, que pode ser usado para fins de autenticação.
    /// </summary>
    /// <param name="user">Os detalhes do usuário para quem o token está sendo gerado.</param>
    /// <returns>Uma string representando o token de autenticação gerado.</returns>
    public string GenerateToken(UserDetailsDTO user);

    /// <summary>
    /// Gera um token de atualização, que pode ser usado para substituir um token expirado ou inválido.
    /// </summary>
    /// <param name="userId">O identificador exclusivo do usuário para quem o token de atualização está sendo gerado.</param>
    /// <param name="securityStamp">Uma assinatura de segurança associada ao usuário para validação adicional.</param>
    /// <returns>Uma string representando o token de atualização gerado.</returns>
    string GenerateRefreshToken(Guid userId, string securityStamp);
}