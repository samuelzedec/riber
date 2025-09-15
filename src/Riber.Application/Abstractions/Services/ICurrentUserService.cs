namespace Riber.Application.Abstractions.Services;

/// <summary>
/// Define um serviço para acessar informações relacionadas ao usuário atual.
/// Este serviço é normalmente usado para recuperar detalhes como identidade do usuário, funções ou dados da sessão.
/// </summary>
/// <remarks>
/// Este serviço só deve ser utilizado em casos onde a autenticação já foi realizada,
/// como em controladores ou middlewares que processam requisições autenticadas.
/// </remarks>
public interface ICurrentUserService
{
    /// <summary>
    /// Obtém as permissões associadas ao usuário atual.
    /// </summary>
    /// <returns>Um array de strings representando as permissões do usuário atual.</returns>
    string[] GetPermissions();

    /// <summary>
    /// Obtém o identificador único associado ao usuário atual.
    /// </summary>
    /// <returns>Um objeto Guid opcional representando o identificador do usuário atual.</returns>
    Guid GetUserId();

    /// <summary>
    /// Obtém o identificador único da empresa associado ao usuário atual.
    /// </summary>
    /// <returns>Um objeto Guid representando o identificador da empresa do usuário atual.</returns>
    Guid GetCompanyId();
}