using Riber.Domain.Abstractions.MultiTenant;

namespace Riber.Domain.Entities.Tenants;

public abstract class OptionalTenantEntity(Guid id) 
    : BaseEntity(id), IOptionalTenantEntity
{
    public Guid? CompanyId { get; protected set; }
}