namespace Riber.Application.Abstractions.Services.Authentication;

public interface IRoleManagementService
{
    /// <summary>
    /// Atribui uma função (role) específica a um usuário existente.
    /// </summary>
    /// <param name="userId">Identificador único do usuário.</param>
    /// <param name="roleName">Nome da função (role) a ser atribuída.</param>
    /// <returns><c>true</c> se a atribuição for bem-sucedida; caso contrário, <c>false</c>.</returns>
    Task<bool> AssignRoleAsync(string userId, string roleName);

    /// <summary>
    /// Substitui todas as funções (roles) atuais de um usuário por uma nova função.
    /// </summary>
    /// <param name="userId">Identificador único do usuário.</param>
    /// <param name="newRole">Nome da nova função (role) que substituirá as existentes.</param>
    /// <returns><c>true</c> se a atualização for bem-sucedida; caso contrário, <c>false</c>.</returns>
    Task<bool> UpdateUserRoleAsync(string userId, string newRole);
}