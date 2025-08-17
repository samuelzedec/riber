namespace SnackFlow.Application.Abstractions.Services;

/// <summary>
/// Define um serviço para acessar informações relacionadas ao usuário atual.
/// Este serviço é normalmente usado para recuperar detalhes como identidade do usuário, funções ou dados da sessão.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Obtém as permissões associadas ao usuário atual.
    /// </summary>
    /// <returns>Um array de strings representando as permissões do usuário atual.</returns>
    public string[] GetPermissions();

    /// <summary>
    /// Obtém o identificador único associado ao usuário atual.
    /// </summary>
    /// <returns>Um objeto Guid opcional representando o identificador do usuário atual ou null se não estiver disponível.</returns>
    public Guid? GetUserId();
}