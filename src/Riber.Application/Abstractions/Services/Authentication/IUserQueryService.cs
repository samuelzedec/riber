using Riber.Application.Common;
using Riber.Application.Models.User;

namespace Riber.Application.Abstractions.Services.Authentication;

public interface IUserQueryService
{
    /// <summary>
    /// Busca um usuário pelo seu identificador único.
    /// </summary>
    /// <param name="userId">Identificador único do usuário.</param>
    /// <returns>Detalhes do usuário encontrado ou erro se não existir.</returns>
    Task<Result<UserDetailsModel>> FindByIdAsync(Guid userId);

    /// <summary>
    /// Busca um usuário pelo endereço de email.
    /// </summary>
    /// <param name="email">Endereço de email do usuário.</param>
    /// <returns>Detalhes do usuário encontrado ou erro se não existir.</returns>
    Task<Result<UserDetailsModel>> FindByEmailAsync(string email);

    /// <summary>
    /// Busca um usuário pelo nome de usuário.
    /// </summary>
    /// <param name="userName">Nome de usuário.</param>
    /// <returns>Detalhes do usuário encontrado ou erro se não existir.</returns>
    Task<Result<UserDetailsModel>> FindByUserNameAsync(string userName);

    /// <summary>
    /// Busca um usuário pelo número de telefone.
    /// </summary>
    /// <param name="phoneNumber">Número de telefone do usuário.</param>
    /// <returns>Detalhes do usuário encontrado ou erro se não existir.</returns>
    Task<Result<UserDetailsModel>> FindByPhoneAsync(string phoneNumber);
}