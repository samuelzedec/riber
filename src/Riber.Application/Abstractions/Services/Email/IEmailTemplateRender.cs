namespace Riber.Application.Abstractions.Services.Email;

/// <summary>
/// Serviço responsável pela renderização de templates de email.
/// </summary>
public interface IEmailTemplateRender
{
    /// <summary>
    /// Gera um template de e-mail processado, renderizando dados em um template especificado.
    /// </summary>
    /// <param name="templatePath">O caminho do arquivo ou identificador do template a ser processado.</param>
    /// <param name="data">Um dicionário contendo pares chave-valor para os dados a serem renderizados no template.</param>
    /// <returns>Uma task representando a operação assíncrona. O resultado da task contém o template de e-mail renderizado como string.</returns>
    Task<string> GetTemplateAsync(string templatePath, Dictionary<string, object?> data);
}