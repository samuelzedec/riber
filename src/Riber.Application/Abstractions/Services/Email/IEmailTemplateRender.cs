using Newtonsoft.Json.Linq;

namespace Riber.Application.Abstractions.Services.Email;

/// <summary>
/// Serviço responsável pela renderização de templates de email.
/// </summary>
public interface IEmailTemplateRender
{
    /// <summary>
    /// Renderiza um template de email substituindo as variáveis pelos dados fornecidos.
    /// </summary>
    /// <param name="data">Dados dinâmicos para substituição no template.</param>
    /// <returns>Template de email renderizado como string.</returns>
    Task<string> GetTemplateAsync(JObject data);
}