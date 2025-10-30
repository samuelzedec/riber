using Riber.Application.Common;
using Riber.Application.Dtos.User;

namespace Riber.Application.Abstractions.Services;

/// <summary>
/// Fornece funcionalidade para criar usuários de forma assíncrona.
/// </summary>
public interface IUserCreationService
{
    /// <summary>
    /// Cria um usuário completo no sistema, incluindo entidade de domínio e conta de aplicação.
    /// </summary>
    /// <param name="dto">Dados necessários para criação do usuário completo.</param>
    /// <param name="cancellationToken">Token para cancelamento da operação.</param>
    /// <returns>Resultado da operação de criação do usuário.</returns>
    Task<Result<EmptyResult>> CreateCompleteUserAsync(CreateUserCompleteDto dto, CancellationToken cancellationToken = default);
}