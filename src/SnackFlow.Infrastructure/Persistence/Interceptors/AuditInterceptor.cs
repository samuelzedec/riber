using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SnackFlow.Domain.Abstractions;
using SnackFlow.Domain.Entities;

namespace SnackFlow.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptador responsável por manipular operações relacionadas à auditoria ao salvar alterações no Entity Framework Core.
/// </summary>
/// <remarks>
/// Esta classe estende o <see cref="SaveChangesInterceptor"/> e sobrescreve os métodos
/// <see cref="SavingChanges"/> e <see cref="SavingChangesAsync"/> para aplicar lógica de auditoria às entidades
/// durante o processo de salvamento.
/// </remarks>
/// <threadsafety>
/// Esta classe é thread-safe e pode ser usada simultaneamente em aplicações onde múltiplas operações de salvamento
/// são executadas em paralelo.
/// </threadsafety>
public sealed class AuditInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            ApplyAuditEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            ApplyAuditEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAuditEntities(DbContext context)
    {
        var entries = context.ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.Entity.UpdateEntity();
                    break;
                    
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.DeleteEntity();
                    break;
            }
        }
    }
}