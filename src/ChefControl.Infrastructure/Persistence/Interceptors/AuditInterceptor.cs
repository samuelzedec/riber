using System.Data.Common;
using ChefControl.Domain.SharedContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ChefControl.Infrastructure.Persistence.Interceptors;

public sealed class AuditInterceptor : DbCommandInterceptor
{
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
        CommandEventData eventData, InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            ApplyAuditEntities(eventData.Context);

        return base.ReaderExecutingAsync(
            command,
            eventData,
            result,
            cancellationToken
        );
    }

    private void ApplyAuditEntities(DbContext context)
    {
        var entries = context.ChangeTracker.Entries<Entity>();
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