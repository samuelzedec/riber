using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Services.AI;
using Riber.Application.Exceptions;

namespace Riber.Infrastructure.Services.AI;

public sealed class EmbeddingsService(
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    ILogger<EmbeddingsService> logger)
    : IEmbeddingsService
{
    public async Task<float[]> GenerateEmbeddingsAsync(
        string input,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var embeddings = await embeddingGenerator.GenerateAsync(input, cancellationToken: cancellationToken);
            return embeddings.Vector.ToArray();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Erro ao gerar embeddings para {Input}.", input);
            throw new InternalException("Erro ao gerar embedding.");
        }
    }

    public async Task<ReadOnlyMemory<float[]>> GenerateEmbeddingsAsync(string[] inputs,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (inputs.Length == 0)
            {
                logger.LogInformation("Nenhum texto para gerar embeddings.");
                return ReadOnlyMemory<float[]>.Empty;
            }

            var embeddings = await embeddingGenerator.GenerateAsync(
                inputs,
                cancellationToken: cancellationToken);

            var result = embeddings
                .Select(e => e.Vector.ToArray())
                .ToArray();

            return new ReadOnlyMemory<float[]>(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Erro ao gerar embeddings em lote para {InputCount} textos.", inputs.Length);
            throw new InternalException("Erro ao gerar embeddings em lote.");
        }
    }
}