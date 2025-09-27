using Riber.Application.DTOs;

namespace Riber.Application.Abstractions.Services;

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
    /// Realiza a autenticação de um usuário com base nas credenciais fornecidas.
    /// </summary>
    /// <param name="email">O email associado à conta do usuário.</param>
    /// <param name="password">A senha correspondente ao usuário.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona, contendo os detalhes do usuário autenticado ou null se as credenciais forem inválidas.</returns>
    Task<UserDetailsDTO?> LoginAsync(string email, string password);

    /// <summary>
    /// Localiza um usuário no sistema com base no ID fornecido.
    /// </summary>
    /// <param name="userId">O identificador único do usuário que está sendo procurado.</param>
    /// <returns>Uma tarefa que retorna os detalhes do usuário encontrado, ou null se o usuário não existir.</returns>
    Task<UserDetailsDTO?> FindByIdAsync(string userId);

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
    /// Atribui uma função específica a um usuário no sistema.
    /// </summary>
    /// <param name="userId">O identificador único do usuário ao qual a função será atribuída.</param> 
    /// <param name="roleName">O nome da função que será atribuída ao usuário.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona de atribuição de função ao usuário.</returns>
    Task AssignRoleToUserAsync(string userId, string roleName);

    /// <summary>
    /// Atualiza o papel atribuído a um usuário com base no identificador do usuário e no novo papel fornecido.
    /// </summary>
    /// <param name="userId">O identificador único do usuário cujo papel será atualizado.</param>
    /// <param name="newRole">O novo papel a ser atribuído ao usuário.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona de atualização do papel do usuário.</returns>
    Task UpdateUserRoleAsync(string userId, string newRole);

    /// <summary>
    /// Verifica a existência de uma função com o nome especificado e lança uma exceção se ela não existir.
    /// </summary>
    /// <param name="roleName">O nome da função a ser verificada.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    Task EnsureRoleExistsAsync(string roleName);

    /// <summary>
    /// Atualiza os detalhes de segurança de um usuário, atualizando propriedades ou dados necessários.
    /// </summary>
    /// <param name="userId">O identificador único do usuário cujos detalhes de segurança precisam ser atualizados.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona de atualização dos detalhes de segurança do usuário.</returns>
    Task RefreshUserSecurityAsync(string userId);
}