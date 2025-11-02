namespace Riber.Application.Abstractions.Messaging;

/// <summary>
/// Define o contrato para publicação de mensagens de forma assíncrona em um sistema de mensageria.
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publica uma mensagem de forma assíncrona para um sistema de mensageria.
    /// </summary>
    /// <typeparam name="TMessage">O tipo da mensagem a ser publicada.</typeparam>
    /// <param name="message">A instância da mensagem a ser publicada.</param>
    /// <param name="cancellationToken">Um token que pode ser usado para solicitar o cancelamento da operação.</param>
    /// <returns>Uma task que representa a operação assíncrona de publicação.</returns>
    Task PublishAsync<TMessage>(
        TMessage message,
        CancellationToken cancellationToken = default)
        where TMessage : class;

    /// <summary>
    /// Publica uma mensagem de forma assíncrona para um sistema de mensageria com opções configuráveis.
    /// </summary>
    /// <typeparam name="TMessage">O tipo da mensagem a ser publicada.</typeparam>
    /// <param name="message">A instância da mensagem a ser publicada.</param>
    /// <param name="configure">Uma ação que permite configurar o contexto de publicação, como prioridade e cabeçalhos.</param>
    /// <param name="cancellationToken">Um token que pode ser usado para solicitar o cancelamento da operação.</param>
    /// <returns>Uma task que representa a operação assíncrona de publicação.</returns>
    Task PublishAsync<TMessage>(
        TMessage message,
        Action<IPublishContext>? configure,
        CancellationToken cancellationToken = default)
        where TMessage : class;
}