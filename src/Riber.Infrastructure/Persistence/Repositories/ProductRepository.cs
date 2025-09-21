using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Riber.Domain.Entities;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.Core;
using Riber.Domain.Specifications.Product;
using Riber.Infrastructure.Persistence.Extensions;

namespace Riber.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository(AppDbContext context)
    : BaseRepository<Product>(context), IProductRepository
{
    private readonly DbSet<ProductCategory> _productCategories = context.Set<ProductCategory>();
    
    public async Task CreateCategoryAsync(ProductCategory productCategory, CancellationToken cancellationToken = default)
        => await _productCategories.AddAsync(productCategory, cancellationToken);
    
    public void UpdateCategory(ProductCategory productCategory)
        => _productCategories.Update(productCategory);
    
    public async Task<ProductCategory?> GetCategoryAsync(
        Specification<ProductCategory> specification,
        CancellationToken cancellationToken = default,
        params Expression<Func<ProductCategory, object>>[] includes)
    {
        var query = _productCategories.GetQueryWithIncludes(specification, includes);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductCategory>> GetCategoriesAsync(
        Specification<ProductCategory> specification,
        CancellationToken cancellationToken = default,
        params Expression<Func<ProductCategory, object>>[] includes)
    {
        var query = _productCategories.GetQueryWithIncludes(specification, includes);
        return await query.ToListAsync(cancellationToken);   
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var queryable = new ProductByCategoryIdSpecification(categoryId).Apply(Table);
        return await queryable.ToListAsync(cancellationToken);
    }
}