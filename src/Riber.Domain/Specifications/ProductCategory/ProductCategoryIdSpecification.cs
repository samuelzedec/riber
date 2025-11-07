using System.Linq.Expressions;
using Riber.Domain.Specifications.Core;

namespace Riber.Domain.Specifications.ProductCategory;

public sealed class ProductCategoryIdSpecification(Guid categoryId)
    : Specification<Entities.Catalog.ProductCategory>
{
    public override Expression<Func<Entities.Catalog.ProductCategory, bool>> ToExpression()
        => entity => entity.Id == categoryId;
}