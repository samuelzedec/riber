using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using Riber.Domain.Entities.Catalog;
using Riber.Infrastructure.Persistence;
using Riber.Infrastructure.Persistence.Models.Embeddings;

namespace Riber.Infrastructure.Services.AI.Models;

public sealed class AiProductService(AppDbContext context)
    : AiModelService<ProductEmbeddingsModel, Product>(context)
{
    public override async Task<ReadOnlyMemory<Product>> FindSimilarAsync(
        float[] query,
        CancellationToken cancellationToken)
    {
        var products = await _table
            .AsNoTracking()
            .Include(x => x.Product)
            .OrderBy(x => x.Embeddings.CosineDistance(new Vector(query)))
            .Take(3)
            .Select(x => x.Product)
            .ToArrayAsync(cancellationToken);

        return new ReadOnlyMemory<Product>(products);
    }

    public override async Task<ProductEmbeddingsModel?> GetByEntityIdAsync(
        Guid entityId,
        CancellationToken cancellationToken)
        => await _table.LastOrDefaultAsync(p => p.ProductId == entityId, cancellationToken);
}