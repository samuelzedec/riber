using SnackFlow.Application.DTOs;

namespace SnackFlow.Application.Abstractions.Services;

/// <summary>
/// Fornece métodos para manipulação de autenticação e operações de gerenciamento de usuários, como criar usuários,
/// atribuir funções e garantir a existência de funções.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Cria um novo usuário da aplicação com base nos detalhes fornecidos.
    /// </summary>
    /// <param name="applicationUser">Os detalhes do usuário a ser criado.</param>
    /// <param name="cancellationToken">Um token para observar a solicitação de cancelamento.</param>
    /// <returns>Uma tarefa que retorna o identificador único do usuário criado.</returns>
    Task<Guid> CreateAsync(CreateApplicationUserDTO applicationUser, CancellationToken cancellationToken = default);

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
    /// Atribui uma função específica a um usuário identificado pelo ID fornecido.
    /// </summary>
    /// <param name="userId">O identificador único do usuário ao qual a função será atribuída.</param>
    /// <param name="role">O nome da função a ser atribuída ao usuário.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona de atribuição de função.</returns>
    Task AssignRoleToUserAsync(string userId, string role);

    /// <summary>
    /// Verifica a existência de uma função com o nome especificado e lança uma exceção se ela não existir.
    /// </summary>
    /// <param name="roleName">O nome da função a ser verificada.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    Task EnsureRoleExistsAsync(string roleName);
}