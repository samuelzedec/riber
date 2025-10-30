using Riber.Application.Dtos.User;

namespace Riber.Application.Abstractions.Services.Authentication;

public interface IUserQueryService
{
    /// <summary>
    /// Obtém um usuário com base em seu identificador exclusivo.
    /// </summary>
    /// <param name="userId">Identificador exclusivo do usuário.</param>
    /// <returns>
    /// Um <see cref="UserDetailsDto"/> contendo os detalhes do usuário, 
    /// ou <c>null</c> caso o usuário não seja encontrado.
    /// </returns>
    Task<UserDetailsDto?> FindByIdAsync(Guid userId);

    /// <summary>
    /// Obtém um usuário com base em seu endereço de e-mail.
    /// </summary>
    /// <param name="email">Endereço de e-mail do usuário.</param>
    /// <returns>
    /// Um <see cref="UserDetailsDto"/> contendo os detalhes do usuário, 
    /// ou <c>null</c> caso o usuário não seja encontrado.
    /// </returns>
    Task<UserDetailsDto?> FindByEmailAsync(string email);

    /// <summary>
    /// Obtém um usuário com base em seu nome de usuário.
    /// </summary>
    /// <param name="userName">Nome de usuário.</param>
    /// <returns>
    /// Um <see cref="UserDetailsDto"/> contendo os detalhes do usuário, 
    /// ou <c>null</c> caso o usuário não seja encontrado.
    /// </returns>
    Task<UserDetailsDto?> FindByUserNameAsync(string userName);

    /// <summary>
    /// Obtém um usuário com base em seu número de telefone.
    /// </summary>
    /// <param name="phoneNumber">Número de telefone do usuário.</param>
    /// <returns>
    /// Um <see cref="UserDetailsDto"/> contendo os detalhes do usuário, 
    /// ou <c>null</c> caso o usuário não seja encontrado.
    /// </returns>
    Task<UserDetailsDto?> FindByPhoneAsync(string phoneNumber);

}