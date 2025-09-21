using System.Linq.Expressions;
using Riber.Domain.Abstractions.MultiTenant;
using Riber.Domain.Specifications.Core;

namespace Riber.Domain.Specifications.Tenants;

public sealed class NoTenantSpecification<T> : Specification<T>
    where T : class, IOptionalTenantEntity
{
    public override Expression<Func<T, bool>> ToExpression()
        => entity => !entity.CompanyId.HasValue;
}