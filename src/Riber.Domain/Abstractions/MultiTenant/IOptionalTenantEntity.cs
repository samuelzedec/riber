namespace Riber.Domain.Abstractions.MultiTenant;

/// <summary>
/// Representa uma entidade que pode estar opcionalmente associada a um tenant dentro
/// de um sistema multi-tenant. O relacionamento com o tenant é indicado através de
/// um TenantId anulável que, quando preenchido, indica o tenant ao qual a entidade
/// pertence. Entidades que implementam esta interface podem residir tanto em contextos
/// específicos de tenant quanto em contextos não específicos de tenant, com base na
/// presença ou ausência de um TenantId.
/// </summary>
public interface IOptionalTenantEntity
{
    Guid? CompanyId { get; }
}