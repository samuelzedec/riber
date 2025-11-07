using System.Linq.Expressions;
using Riber.Domain.Specifications.Core;

namespace Riber.Domain.Specifications.Product;

public sealed class ProductByIdSpecification(Guid id)
    : Specification<Entities.Catalog.Product>
{
    public override Expression<Func<Entities.Catalog.Product, bool>> ToExpression()
        => product => product.Id == id;
}