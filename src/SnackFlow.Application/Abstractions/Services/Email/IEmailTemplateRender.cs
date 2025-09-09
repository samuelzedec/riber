using Newtonsoft.Json.Linq;

namespace SnackFlow.Application.Abstractions.Services.Email;

/// <summary>
/// Interface de serviço para manipulação de templates de email.
/// Fornece métodos para recuperar e processar templates de email com base em parâmetros especificados.
/// </summary>
public interface IEmailTemplateRender
{
    /// <summary>
    /// Recupera um template de email com base no público-alvo, tipo de template e contexto de dados especificados.
    /// </summary>
    /// <param name="audience">O público-alvo do template de email (ex: Usuário ou Empresa).</param>
    /// <param name="template">O tipo de template de email a ser recuperado (ex: Boas-vindas, Redefinição de Senha).</param>
    /// <param name="data">Os dados dinâmicos a serem populados no template de email.</param>
    /// <returns>Uma task que representa a operação assíncrona. O resultado da task contém o template de email populado como uma string.</returns>
    Task<string> GetTemplateAsync(JObject data);
}