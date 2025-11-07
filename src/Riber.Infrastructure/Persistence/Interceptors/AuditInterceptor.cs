using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Riber.Domain.Abstractions;
using Riber.Domain.Entities;
using Riber.Domain.Entities.Abstractions;
using Riber.Domain.Entities.User;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Um interceptador para auditoria de mudanças de entidades durante uma operação de `SaveChanges` no banco de dados.
/// Esta classe estende o `SaveChangesInterceptor` do Entity Framework Core para se conectar
/// ao processo de salvamento para rastreamento ou modificação de entidades antes da persistência.
/// </summary>
/// <remarks>
/// O AuditInterceptor foi projetado para capturar e aplicar lógica de auditoria, como
/// rastreamento de timestamps de criação ou modificação, durante operações de salvamento. Ele intercepta
/// fluxos de trabalho de salvamento síncronos e assíncronos no contexto do Entity Framework Core.
/// </remarks>
public sealed class AuditInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            ApplyAuditTracker(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            ApplyAuditTracker(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Aplica lógica de auditoria às trackers rastreadas pelo contexto do Entity Framework Core.
    /// Atualiza os timestamps de modificação ou realiza ações específicas para estados de entidade,
    /// como exclusões suaves.
    /// </summary>
    /// <param name="context">O contexto do banco de dados que contém o rastreador de alterações para as entidades alvo.</param>
    private static void ApplyAuditTracker(DbContext context)
    {
        var entries = context.ChangeTracker.Entries<Tracker>();
        foreach (var entry in entries)
        {
            if (entry.State is EntityState.Modified)
                entry.Entity.UpdateEntity();

            if (entry.State is EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.DeleteEntity();
                
                if (entry.Entity is User userDomain)
                    DeactivateApplicationUser(context, userDomain.Id);
            }
        }
    }

    /// <summary>
    /// Desativa um usuário da aplicação marcando-o como excluído e define
    /// o estado da entidade como Modificado no contexto do banco de dados fornecido.
    /// </summary>
    /// <param name="context">O contexto do banco de dados usado para acessar e modificar a entidade do usuário da aplicação.</param>
    /// <param name="userDomainId">O identificador único do usuário no domínio a ser desativado.</param>
    private static void DeactivateApplicationUser(DbContext context, Guid userDomainId)
    {
        // Verifica primeiro se está no Change Tracker, se não irá fazer a consulta no banco
        var applicationUser = context
            .ChangeTracker.Entries<ApplicationUser>().FirstOrDefault(e => e.Entity.UserDomainId == userDomainId)?.Entity 
            ?? context.Set<ApplicationUser>().FirstOrDefault(u => u.UserDomainId == userDomainId);

        if (applicationUser is null)
            return;

        applicationUser.IsDeleted = true;
        context.Entry(applicationUser).State = EntityState.Modified;
    }
}