using Riber.Application.Common;

namespace Riber.Application.Abstractions.Services.Authentication;

public interface IRoleManagementService
{
    /// <summary>
    /// Atribui uma role específica a um usuário.
    /// </summary>
    /// <param name="userId">Identificador único do usuário.</param>
    /// <param name="roleName">Nome da role a ser atribuída.</param>
    /// <returns>Resultado da operação de atribuição de role.</returns>
    Task<Result<EmptyResult>> AssignRoleAsync(string userId, string roleName);

    /// <summary>
    /// Substitui todas as roles atuais do usuário por uma nova role.
    /// </summary>
    /// <param name="userId">Identificador único do usuário.</param>
    /// <param name="newRole">Nome da nova role que substituirá as existentes.</param>
    /// <returns>Resultado da operação de atualização de role.</returns>
    Task<Result<EmptyResult>> UpdateUserRoleAsync(string userId, string newRole);

    /// <summary>
    /// Verifica se uma role existe no sistema e lança erro se não existir.
    /// </summary>
    /// <param name="roleName">Nome da role a ser verificada.</param>
    /// <returns>Resultado da operação de verificação.</returns>
    Task<Result<EmptyResult>> EnsureRoleExistsAsync(string roleName);
}