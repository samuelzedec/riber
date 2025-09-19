namespace Riber.Domain.Abstractions.MultiTenant;

/// <summary>
/// Define o contrato para entidades que estão associadas a um tenant, identificadas por um CompanyId.
/// Esta interface é parte do suporte a multi-tenant dentro das abstrações do domínio. 
/// Implementar esta interface permite que as entidades sejam associadas a um tenant específico.
/// </summary>
public interface ITenantEntity
{
    Guid CompanyId { get; }
}