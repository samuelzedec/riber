using Riber.Application.Models.User;

namespace Riber.Application.Abstractions.Services.Authentication;

public interface IAuthenticationService
{
    /// <summary>
    /// Realiza a autenticação de um usuário com base no nome de usuário ou e-mail e na senha informada.
    /// </summary>
    /// <param name="userNameOrEmail">Nome de usuário ou endereço de e-mail.</param>
    /// <param name="password">Senha correspondente ao usuário.</param>
    /// <returns>Modelo com os detalhes do usuário autenticado ou <c>null</c> em caso de falha na autenticação.</returns>
    Task<UserDetailsModel?> LoginAsync(string userNameOrEmail, string password);

    /// <summary>
    /// Atualiza o *security stamp* do usuário, invalidando quaisquer tokens de autenticação existentes.
    /// </summary>
    /// <param name="userId">Identificador único do usuário.</param>
    /// <returns><c>true</c> se a atualização foi bem-sucedida; caso contrário, <c>false</c>.</returns>
    Task<bool> RefreshSecurityStampAsync(string userId);
}