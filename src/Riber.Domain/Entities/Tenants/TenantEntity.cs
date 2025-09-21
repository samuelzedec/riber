using Riber.Domain.Abstractions.MultiTenant;

namespace Riber.Domain.Entities.Tenants;

public abstract class TenantEntity(Guid id) 
    : BaseEntity(id), ITenantEntity
{
    public Guid CompanyId { get; protected set; }
}