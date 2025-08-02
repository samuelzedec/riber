using System.Security.Claims;
using SnackFlow.Application.DTOs;
using SnackFlow.Domain.Enums;

namespace SnackFlow.Application.Abstractions.Services;

/// <summary>
/// Fornece métodos para manipulação de autenticação e operações de gerenciamento de usuários, como criar usuários,
/// atribuir funções e garantir a existência de funções.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Cria um novo usuário no sistema com base nos dados fornecidos.
    /// </summary>
    /// <param name="applicationUser">Os dados do usuário que será criado, incluindo nome, email, senha e outras informações.</param>
    /// <param name="cancellationToken">O token de cancelamento que pode ser usado para interromper a operação.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona de criação do usuário.</returns>
    Task CreateAsync(CreateApplicationUserDTO applicationUser, CancellationToken cancellationToken);

    /// <summary>
    /// Realiza o login de um usuário no sistema com base nas credenciais fornecidas.
    /// </summary>
    /// <param name="email">O endereço de email associado à conta do usuário.</param>
    /// <param name="password">A senha da conta do usuário.</param>
    /// <returns>Uma tarefa que retorna um valor booleano indicando se o login foi bem-sucedido.</returns>
    Task<bool> LoginAsync(string email, string password);

    /// <summary>
    /// Localiza um usuário no sistema com base no ID fornecido.
    /// </summary>
    /// <param name="userId">O identificador único do usuário que está sendo procurado.</param>
    /// <returns>Uma tarefa que retorna os detalhes do usuário encontrado, ou null se o usuário não existir.</returns>
    Task<UserDetailsDTO?> FindByIdAsync(Guid userId);

    /// <summary>
    /// Busca um usuário no sistema com base no endereço de e-mail fornecido.
    /// </summary>
    /// <param name="email">O endereço de e-mail do usuário que está sendo pesquisado.</param>
    /// <returns>Um objeto <see cref="UserDetailsDTO"/> contendo informações detalhadas do usuário se encontrado, ou null se o usuário não existir.</returns>
    Task<UserDetailsDTO?> FindByEmailAsync(string email);

    /// <summary>
    /// Busca um usuário no sistema com base no nome de usuário fornecido.
    /// </summary>
    /// <param name="userName">O nome de usuário do usuário que está sendo pesquisado no sistema.</param>
    /// <returns>Uma tarefa que, quando concluída, contém as informações detalhadas do usuário na forma de um <see cref="UserDetailsDTO"/>, ou null se o usuário não for encontrado.</returns>
    Task<UserDetailsDTO?> FindByUserNameAsync(string userName);

    /// <summary>
    /// Busca e retorna os detalhes de um usuário com base no número de telefone fornecido.
    /// </summary>
    /// <param name="phoneNumber">O número de telefone do usuário a ser buscado.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado contém os detalhes do usuário, ou null se nenhum usuário for encontrado.</returns>
    Task<UserDetailsDTO?> FindByPhoneAsync(string phoneNumber);

    /// <summary>
    /// Atribui uma função ao usuário com base na posição empresarial especificada.
    /// </summary>
    /// <param name="userId">O identificador único do usuário para atribuir a função.</param>
    /// <param name="position">A posição empresarial que corresponde à função a ser atribuída.</param>
    /// <returns>Uma tarefa representando a operação assíncrona.</returns>
    Task AssignRoleToUserAsync(string userId, BusinessPosition position);

    /// <summary>
    /// Atualiza a função do usuário com base na nova posição empresarial especificada.
    /// </summary>
    /// <param name="userId">O identificador único do usuário cujo papel deve ser atualizado.</param>
    /// <param name="newPosition">A nova posição empresarial que define a função a ser atribuída ao usuário.</param>
    /// <returns>Uma tarefa representando a operação assíncrona.</returns>
    Task UpdateUserRoleAsync(string userId, BusinessPosition newPosition);

    /// <summary>
    /// Verifica a existência de uma função com o nome especificado e lança uma exceção se ela não existir.
    /// </summary>
    /// <param name="roleName">O nome da função a ser verificada.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    Task EnsureRoleExistsAsync(string roleName);

    /// <summary>
    /// Retorna uma lista de declarações de direitos (claims) associadas a um determinado nome de função.
    /// </summary>
    /// <param name="roleName">O nome da função para a qual as declarações de direitos devem ser obtidas.</param>
    /// <returns>Uma tarefa que retorna uma lista de objetos <see cref="Claim"/> associados à função especificada.</returns>
    Task<IList<Claim>> GetClaimsByRoleName(string roleName);
}