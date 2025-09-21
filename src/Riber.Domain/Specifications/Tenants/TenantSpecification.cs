using System.Linq.Expressions;
using Riber.Domain.Abstractions.MultiTenant;
using Riber.Domain.Specifications.Core;

namespace Riber.Domain.Specifications.Tenants;

public sealed class TenantSpecification<T>(Guid id) : Specification<T>
    where T : class, ITenantEntity
{
    public override Expression<Func<T, bool>> ToExpression()
        => entity => entity.CompanyId == id;
}