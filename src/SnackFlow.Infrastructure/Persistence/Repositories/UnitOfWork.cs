using Mediator;
using Microsoft.EntityFrameworkCore.Storage;
using SnackFlow.Domain.Entities;
using SnackFlow.Domain.Repositories;

namespace SnackFlow.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementa o padrão unit of work, que gerencia conexões com o banco de dados 
/// e garante consistência entre operações agrupando mudanças em transações.
/// </summary>
public sealed class UnitOfWork(AppDbContext context, IMediator mediator) 
    : IUnitOfWork, IAsyncDisposable, IDisposable
{
    #region Attributes

    private IDbContextTransaction? _transaction;
    private ICompanyRepository? _companyRepository;
    private IUserRepository? _userRepository;
    private IInvitationRepository? _invitationRepository;

    #endregion
    
    #region Properties

    public ICompanyRepository Companies
        => _companyRepository ??= new CompanyRepository(context);
    
    public IUserRepository Users
        => _userRepository ??= new UserRepository(context);
    
    public IInvitationRepository Invitations
        => _invitationRepository ??= new InvitationRepository(context);
    
    #endregion

    #region Default Methods
    
    public async Task SaveAsync(CancellationToken cancellationToken = default)
        => await SaveChangesAndPublishEventsAsync(cancellationToken);
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await SaveChangesAndPublishEventsAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        => _transaction ??= await context.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if(_transaction is not null)
        {
            await SaveChangesAndPublishEventsAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
            await _transaction.DisposeAsync();
        await context.DisposeAsync();
    }
    
    public void Dispose()
    {
        _transaction?.Dispose();
        context.Dispose();
    }

    #endregion

    #region Private Methods

    private async Task<int> SaveChangesAndPublishEventsAsync(CancellationToken cancellationToken = default)
    {
        var entitiesWithEvents = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.Events().Count != 0)
            .ToList();

        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.Entity.Events())
            .ToList();
        
        entitiesWithEvents.ForEach(e => e.Entity.ClearEvents());
        var result = await context.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent, cancellationToken);

        return result;
    }

    #endregion
}