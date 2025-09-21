using System.Linq.Expressions;
using Riber.Domain.Specifications.Core;

namespace Riber.Domain.Specifications.ProductCategory;

public sealed class ProductCategoryCodeSpecification(string code)
    : Specification<Entities.ProductCategory>
{
    public override Expression<Func<Entities.ProductCategory, bool>> ToExpression()
        => entity => entity.Code == code;
}