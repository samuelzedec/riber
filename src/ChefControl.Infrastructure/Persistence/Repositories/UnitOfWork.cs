using ChefControl.Domain.CompanyContext.Repositories;
using ChefControl.Domain.SharedContext.Abstractions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ChefControl.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementa o padrão unit of work, que gerencia conexões com o banco de dados 
/// e garante consistência entre operações agrupando mudanças em transações.
/// </summary>
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    #region Attributes

    private IDbContextTransaction? _transaction;
    private ICompanyRepository? _companyRepository;

    #endregion
    
    #region Properties

    public ICompanyRepository Companies
        => _companyRepository ??= new CompanyRepository(context);

    #endregion

    #region Default Methods

    public async Task SaveAsync(CancellationToken cancellationToken = default)
        => await context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        => _transaction ??= await context.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if(_transaction is not null)
        {
            await context.SaveChangesAsync(cancellationToken);
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
}