using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Riber.Domain.Entities.Catalog;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.Core;
using Riber.Domain.Specifications.Image;
using Riber.Infrastructure.Extensions;

namespace Riber.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository(AppDbContext context)
    : BaseRepository<Product>(context), IProductRepository
{
    #region Tables

    private readonly DbSet<ProductCategory> _productCategories = context.Set<ProductCategory>();
    private readonly DbSet<Image> _images = context.Set<Image>();

    #endregion

    #region Category Methods

    public async Task CreateCategoryAsync(ProductCategory productCategory,
        CancellationToken cancellationToken = default)
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

    #endregion

    #region Image Methods

    public async Task CreateImageAsync(Image image, CancellationToken cancellationToken = default)
        => await _images.AddAsync(image, cancellationToken);

    public async Task<IReadOnlyList<Image>> GetUnusedImagesAsync(CancellationToken cancellationToken = default)
        => await _images
            .AsNoTracking()
            .Where(new ImagesReadyForCleanupSpecification())
            .ToListAsync(cancellationToken);

    public void DeleteImage(Image image)
        => _images.Remove(image);

    #endregion
}