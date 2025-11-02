namespace Riber.Application.Abstractions.Messaging;

/// <summary>
/// Representa um contexto para configurar parâmetros de publicação de mensagens como prioridade, atraso e cabeçalhos.
/// </summary>
public interface IPublishContext
{
    /// <summary>
    /// Define o nível de prioridade da mensagem.
    /// </summary>
    /// <param name="priority">O nível de prioridade a ser atribuído à mensagem.</param>
    void SetPriority(byte priority);

    /// <summary>
    /// Define um tempo de atraso para o processamento da mensagem.
    /// </summary>
    /// <param name="delay">A duração do atraso antes que a mensagem seja processada.</param>
    void SetDelay(TimeSpan delay);

    /// <summary>
    /// Define um cabeçalho com a chave e valor especificados.
    /// </summary>
    /// <param name="key">A chave do cabeçalho a ser definido.</param>
    /// <param name="value">O valor associado à chave especificada.</param>
    void SetHeader(string key, object value);
}