using System.Data.Common;
using SnackFlow.Domain.SharedContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace SnackFlow.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Representa um interceptador que aplica lógica de auditoria durante a execução de comandos do banco de dados.
/// </summary>
/// <remarks>
/// O <c>AuditInterceptor</c> é utilizado para interceptar comandos do banco de dados e aplicar alterações
/// relacionadas à auditoria nas entidades associadas. Isso é particularmente útil para operações que requerem
/// rastreamento ou auditoria no contexto de uma aplicação Entity Framework Core.
/// </remarks>
/// <example>
/// Tipicamente usado ao registrá-lo como um interceptador do contexto DB, o <c>AuditInterceptor</c> se integra
/// perfeitamente com o Entity Framework Core através de injeção de dependência ou durante a configuração do contexto.
/// </example>
/// <threadsafety>
/// Esta classe é thread-safe e pode ser usada em cenários com múltiplos contextos de banco de dados ou operações
/// concorrentes.
/// </threadsafety>
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