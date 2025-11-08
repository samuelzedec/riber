using Riber.Domain.Entities.Catalog;

namespace Riber.Infrastructure.Persistence.Models.Embeddings;

public sealed class ProductEmbeddingsModel
    : BaseEmbeddingsModel
{
    public required Guid ProductId { get; init; }
    public Product Product { get; set; } = null!;

    public static string ToEmbeddingString(Product product)
        => $"""
            Nome: {product.Name}
            Descrição: {product.Description}
            Categoria: {product.Category.Name}
            """;
}