namespace Riber.Application.Messages;

/// <summary>
/// Representa uma mensagem utilizada para gerar embeddings para um produto.
/// Esta mensagem encapsula a entidade do produto para o qual os embeddings precisam ser gerados.
/// </summary>
public sealed record GenerateProductEmbeddingsMessage(Guid ProductId);