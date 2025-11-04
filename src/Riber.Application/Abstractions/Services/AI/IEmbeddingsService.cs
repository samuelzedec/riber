namespace Riber.Application.Abstractions.Services.AI;

/// <summary>
/// Fornece funcionalidade para gerar e utilizar embeddings para operações relacionadas à IA.
/// </summary>
public interface IEmbeddingsService
{
    /// <summary>
    /// Gera embeddings de forma assíncrona a partir da string de entrada especificada.
    /// </summary>
    /// <param name="input">A string de entrada com base na qual os embeddings são gerados.</param>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona, contendo um array de números de ponto flutuante que representam os embeddings gerados.</returns>
    Task<float[]> GenerateEmbeddingsAsync(string input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gera embeddings de forma assíncrona para uma coleção de strings de entrada especificadas.
    /// </summary>
    /// <param name="inputs">Um array de strings de entrada com base nas quais os embeddings são gerados.</param>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona, contendo uma coleção de arrays de números de ponto flutuante que representam os embeddings gerados.</returns>
    Task<ReadOnlyMemory<float[]>> GenerateEmbeddingsAsync(string[] inputs, CancellationToken cancellationToken = default);
}