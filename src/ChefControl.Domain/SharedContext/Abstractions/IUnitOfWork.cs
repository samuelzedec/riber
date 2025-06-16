using ChefControl.Domain.CompanyContext.Repositories;

namespace ChefControl.Domain.SharedContext.Abstractions;

/// <summary>
/// Representa uma unidade de trabalho que encapsula um conjunto de mudanças para persistência.
/// Fornece mecanismos para gerenciar transações e repositórios de forma coesa.
/// </summary>
public interface IUnitOfWork
{
    #region Repositories
    
    ICompanyRepository Companies { get; }
    
    #endregion

    #region Default Methods

    Task SaveAsync(CancellationToken cancellationToken);
    Task BeginTransactionAsync(CancellationToken cancellationToken);
    Task CommitTransactionAsync(CancellationToken cancellationToken);
    Task RollbackTransactionAsync(CancellationToken cancellationToken);
    ValueTask DisposeAsync();
    void Dispose();

    #endregion
}