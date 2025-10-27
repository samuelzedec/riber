namespace Riber.Application.Abstractions.Dispatchers;

/// <summary>
/// Dispatcher responsável por agendar a exclusão de imagens do storage em background.
/// </summary>
public interface IDeleteImageFromStorageDispatcher
{
    /// <summary>
    /// Agenda a remoção de uma imagem do armazenamento em nuvem de forma assíncrona.
    /// A operação é executada em background através de um job agendado.
    /// </summary>
    /// <param name="imageKey">A chave única da imagem a ser removida do storage (ex: "12345.jpg")</param>
    /// <param name="cancellationToken">Token para cancelamento da operação de agendamento</param>
    /// <returns>Uma task que completa quando o job de remoção foi agendado com sucesso</returns>
    Task SendAsync(string imageKey, CancellationToken cancellationToken);
}