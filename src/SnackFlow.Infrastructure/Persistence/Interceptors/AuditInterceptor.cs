using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SnackFlow.Domain.Entities;
using SnackFlow.Infrastructure.Persistence.Identity;

namespace SnackFlow.Infrastructure.Persistence.Interceptors;

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

    /// <summary>
    /// Aplica lógica de auditoria às entidades rastreadas pelo contexto do Entity Framework Core.
    /// Atualiza os timestamps de modificação ou realiza ações específicas para estados de entidade,
    /// como exclusões suaves.
    /// </summary>
    /// <param name="context">O contexto do banco de dados que contém o rastreador de alterações para as entidades alvo.</param>
    private static void ApplyAuditEntities(DbContext context)
    {
        var entries = context.ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            if (entry.State is EntityState.Modified)
                entry.Entity.UpdateEntity();

            if (entry.State is EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.DeleteEntity();
                
                if (entry.Entity is User userDomain)
                    DeactivateApplicationUser(context, userDomain.ApplicationUserId);
            }
        }
    }


    /// <summary>
    /// Marca o usuário do sistema (ApplicationUser) como excluído definindo sua propriedade IsDeleted como verdadeira e
    /// atualiza seu estado no rastreador de mudanças do Entity Framework Core para Modified.
    /// </summary>
    /// <param name="context">O contexto do banco de dados usado para rastrear e acessar a entidade ApplicationUser.</param>
    /// <param name="userApplicationId">O identificador único da entidade ApplicationUser a ser desativada.</param>
    private static void DeactivateApplicationUser(DbContext context, Guid userApplicationId)
    {
        var applicationUser = context
            .ChangeTracker.Entries<ApplicationUser>()
            .FirstOrDefault(e => e.Entity.Id == userApplicationId)?.Entity 
            ?? context.Set<ApplicationUser>().FirstOrDefault(u => u.Id == userApplicationId);
    
        if (applicationUser is null)
            return;
    
        applicationUser.IsDeleted = true;
        context.Entry(applicationUser).State = EntityState.Modified;
    }
}