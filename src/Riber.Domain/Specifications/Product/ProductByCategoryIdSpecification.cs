using System.Linq.Expressions;
using Riber.Domain.Specifications.Core;

namespace Riber.Domain.Specifications.Product;

public sealed class ProductByCategoryIdSpecification(Guid categoryId)
    : Specification<Entities.Catalog.Product>
{
    public override Expression<Func<Entities.Catalog.Product, bool>> ToExpression()
        => entity => entity.CategoryId == categoryId;
}