namespace Riber.Application.Abstractions.Services.Authentication;

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
    /// Retorna as permissões associadas ao usuário atual.
    /// </summary>
    /// <returns>Array de strings com as permissões do usuário.</returns>
    string[] GetPermissions();

    /// <summary>
    /// Retorna o identificador único do usuário atual.
    /// </summary>
    /// <returns>Guid do usuário atual.</returns>
    Guid GetUserId();

    /// <summary>
    /// Retorna o identificador da empresa do usuário atual, se houver.
    /// </summary>
    /// <returns>Guid da empresa ou <c>null</c> se não houver.</returns>
    Guid? GetCompanyId();
}