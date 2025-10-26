using Riber.Application.Common;
using Riber.Application.Models.User;

namespace Riber.Application.Abstractions.Services.Authentication;

public interface IAuthenticationService
{
    /// <summary>
    /// Autentica um usuário usando nome de usuário/email e senha.
    /// </summary>
    /// <param name="userNameOrEmail">Nome de usuário ou endereço de email.</param>
    /// <param name="password">Senha do usuário.</param>
    /// <returns>Detalhes do usuário autenticado ou erro de autenticação.</returns>
    Task<Result<UserDetailsModel>> LoginAsync(string userNameOrEmail, string password);

    /// <summary>
    /// Atualiza o security stamp do usuário para invalidar tokens existentes.
    /// </summary>
    /// <param name="userId">Identificador único do usuário.</param>
    /// <returns>Resultado da operação de atualização.</returns>
    Task<Result<EmptyResult>> RefreshSecurityStampAsync(string userId);
}