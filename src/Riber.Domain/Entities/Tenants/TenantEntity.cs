using Riber.Domain.Abstractions.MultiTenant;
using Riber.Domain.Entities.Abstractions;

namespace Riber.Domain.Entities.Tenants;

public abstract class TenantEntity(Guid id) 
    : BaseEntity(id), ITenantEntity
{
    public Guid CompanyId { get; protected set; }
}