using System.Linq.Expressions;
using Riber.Domain.Specifications.Core;

namespace Riber.Domain.Specifications.Product;

public sealed class ProductByCategoryIdSpecification(Guid categoryId)
    : Specification<Entities.Product>
{
    public override Expression<Func<Entities.Product, bool>> ToExpression()
        => entity => entity.CategoryId == categoryId;
}