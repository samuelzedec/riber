namespace Riber.Application.Abstractions.Services.AI;

public interface IAiModelService<TInput, TOutput>
    where TInput : class
    where TOutput : class
{
    /// <summary>
    /// Cria um recurso ou dado com base no modelo especificado.
    /// </summary>
    /// <param name="model">O modelo que representa o recurso ou dado a ser criado.</param>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona de criação.</returns>
    Task CreateAsync(TInput model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui um recurso ou dado com base no modelo especificado.
    /// </summary>
    /// <param name="model">O modelo que representa o recurso ou dado a ser excluído.</param>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona de exclusão.</returns>
    Task DeleteAsync(TInput model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Encontra itens semelhantes com base no vetor de consulta fornecido.
    /// </summary>
    /// <param name="query">Um array de números de ponto flutuante representando o vetor de consulta para a operação de similaridade.</param>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <typeparam name="TOutput">O tipo de item que será retornado como resultado da operação.</typeparam>
    /// <returns>Uma tarefa que representa a operação assíncrona, contendo uma memória somente leitura de itens semelhantes do tipo especificado.</returns>
    Task<ReadOnlyMemory<TOutput>> FindSimilarAsync(float[] query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um recurso ou dado com base no identificador da entidade especificado.
    /// </summary>
    /// <param name="entityId">O identificador único da entidade a ser recuperada.</param>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona, contendo o recurso ou dado encontrado, ou null se não encontrado.</returns>
    Task<TInput?> GetByEntityIdAsync(Guid entityId, CancellationToken cancellationToken = default);
}