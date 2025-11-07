using System.Linq.Expressions;
using Riber.Domain.Specifications.Core;

namespace Riber.Domain.Specifications.ProductCategory;

public sealed class ProductCategoryCodeSpecification(string code)
    : Specification<Entities.Catalog.ProductCategory>
{
    public override Expression<Func<Entities.Catalog.ProductCategory, bool>> ToExpression()
        => entity => entity.Code == code;
}