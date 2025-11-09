using Riber.Domain.Entities.Catalog;
using Riber.Domain.Entities.Company;

namespace Riber.Infrastructure.Persistence.Models.Embeddings;

public sealed class ProductEmbeddingsModel
    : BaseEmbeddingsModel
{
    public required Guid ProductId { get; init; }
    public required Guid CompanyId { get; init; }
    public Product Product { get; private set; } = null!;
    public Company Company { get; private set; } = null!;

    public static string ToEmbeddingString(Product product)
        => $"""
            Nome: {product.Name}
            Descrição: {product.Description}
            Categoria: {product.Category.Name}
            """;
}