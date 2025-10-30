using Riber.Application.Dtos.User;

namespace Riber.Application.Abstractions.Services.Authentication;

public interface IUserManagementService
{
    /// <summary>
    /// Cria um novo usuário no sistema de identidade com base nos dados fornecidos.
    /// </summary>
    /// <param name="dto">Modelo contendo as informações necessárias para a criação do usuário.</param>
    /// <returns>
    /// <c>true</c> se o usuário for criado com sucesso; caso contrário, <c>false</c>.
    /// </returns>
    Task<bool> CreateUserAsync(CreateApplicationUserDto dto);

    /// <summary>
    /// Remove permanentemente um usuário do sistema de identidade.
    /// </summary>
    /// <param name="userId">Identificador exclusivo do usuário a ser removido.</param>
    /// <returns>
    /// <c>true</c> se a exclusão for bem-sucedida; caso contrário, <c>false</c>.
    /// </returns>
    Task<bool> DeleteUserAsync(string userId);
}