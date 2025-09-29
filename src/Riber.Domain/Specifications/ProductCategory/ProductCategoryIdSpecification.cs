using System.Linq.Expressions;
using Riber.Domain.Specifications.Core;

namespace Riber.Domain.Specifications.ProductCategory;

public sealed class ProductCategoryIdSpecification(Guid categoryId)
    : Specification<Entities.ProductCategory>
{
    public override Expression<Func<Entities.ProductCategory, bool>> ToExpression()
        => entity => entity.Id == categoryId;
}