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
        Guid companyId,
        float[] query,
        CancellationToken cancellationToken = default)
    {
        var products = await _table
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .Include(x => x.Product)
            .OrderBy(x => x.Embeddings.CosineDistance(new Vector(query)))
            .Take(3)
            .Select(x => x.Product)
            .ToArrayAsync(cancellationToken);

        return new ReadOnlyMemory<Product>(products);
    }
}