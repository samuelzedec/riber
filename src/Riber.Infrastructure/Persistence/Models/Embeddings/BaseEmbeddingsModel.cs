using Pgvector;

namespace Riber.Infrastructure.Persistence.Models.Embeddings;

/// <summary>
/// Representa um modelo base para estruturas de embedding usando representação vetorial.
/// Esta classe abstrata serve como fundação para modelos específicos de embedding
/// e inclui uma propriedade vetorial obrigatória para armazenar embeddings.
/// </summary>
public abstract class BaseEmbeddingsModel() : BaseModel(Guid.CreateVersion7())
{
    /// <summary>
    /// Obtém a representação vetorial dos embeddings para o modelo.
    /// Esta propriedade é obrigatória e armazena o vetor de embedding
    /// usando uma estrutura específica. Serve como elemento fundamental
    /// para derivar funcionalidades baseadas em embeddings nos modelos.
    /// </summary>
    public Vector Embeddings { get; set; } = null!;
}