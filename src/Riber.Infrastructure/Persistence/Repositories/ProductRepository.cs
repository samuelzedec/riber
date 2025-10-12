using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Riber.Domain.Entities;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.Core;
using Riber.Domain.Specifications.Image;
using Riber.Infrastructure.Persistence.Extensions;

namespace Riber.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository(AppDbContext context)
    : BaseRepository<Product>(context), IProductRepository
{
    private readonly DbSet<ProductCategory> _productCategories = context.Set<ProductCategory>();
    private readonly DbSet<Image> _images = context.Set<Image>();

    public async Task CreateCategoryAsync(ProductCategory productCategory, CancellationToken cancellationToken = default)
        => await _productCategories.AddAsync(productCategory, cancellationToken);

    public void UpdateCategory(ProductCategory productCategory)
        => _productCategories.Update(productCategory);

    public async Task<ProductCategory?> GetCategoryAsync(
        Specification<ProductCategory> specification,
        CancellationToken cancellationToken = default,
        params Expression<Func<ProductCategory, object>>[] includes)
        => await _productCategories
            .AsNoTracking()
            .GetQueryWithIncludes(specification, includes)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<ProductCategory>> GetCategoriesAsync(
        Specification<ProductCategory> specification,
        CancellationToken cancellationToken = default,
        params Expression<Func<ProductCategory, object>>[] includes)
        => await _productCategories
            .AsNoTracking()
            .GetQueryWithIncludes(specification, includes)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Image>> GetUnusedImagesAsync(CancellationToken cancellationToken = default)
        => await _images
            .AsNoTracking()
            .Where(new ImagesReadyForCleanupSpecification())
            .ToListAsync(cancellationToken);
}