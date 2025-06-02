using ChefControl.Domain.CompanyContext.Repositories;

namespace ChefControl.Domain.SharedContext.Persistence;

/// <summary>
/// Representa uma unidade de trabalho que encapsula um conjunto de mudanças para persistência.
/// Fornece mecanismos para gerenciar transações e repositórios de forma coesa.
/// </summary>
public interface IUnitOfWork
{
    #region Repositories
    
    ICompanyRepository Company { get; }
    
    #endregion

    #region Default Methods

    Task SaveAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();

    #endregion
}