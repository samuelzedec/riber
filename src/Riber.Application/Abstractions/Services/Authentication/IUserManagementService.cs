using Riber.Application.Common;
using Riber.Application.Models.User;

namespace Riber.Application.Abstractions.Services.Authentication;

public interface IUserManagementService
{
    /// <summary>
    /// Cria um novo usuário no sistema de identidade com as informações fornecidas.
    /// </summary>
    /// <param name="model">Dados necessários para criação do usuário de aplicação.</param>
    /// <returns>Resultado da operação de criação do usuário.</returns>
    Task<Result<EmptyResult>> CreateUserAsync(CreateApplicationUserModel model);

    /// <summary>
    /// Remove um usuário do sistema de identidade permanentemente.
    /// </summary>
    /// <param name="userId">Identificador único do usuário a ser removido.</param>
    /// <returns>Resultado da operação de exclusão do usuário.</returns>
    Task<Result<EmptyResult>> DeleteUserAsync(string userId);
}