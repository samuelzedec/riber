using Riber.Domain.Abstractions.MultiTenant;
using Riber.Domain.Entities.Abstractions;

namespace Riber.Domain.Entities.Tenants;

public abstract class OptionalTenantEntity(Guid id) 
    : BaseEntity(id), IOptionalTenantEntity
{
    public Guid? CompanyId { get; protected set; }
}