namespace Riber.Domain.Repositories;

/// <summary>
/// Representa uma unidade de trabalho que encapsula um conjunto de mudanças para persistência.
/// Fornece mecanismos para gerenciar transações e repositórios de forma coesa.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable, IDisposable
{
    #region Repositories
    
    ICompanyRepository Companies { get; }
    
    IUserRepository Users { get; }
    
    IInvitationRepository Invitations { get; }
    
    IOrderRepository Orders { get; }
    
    IProductRepository Products { get; }
    
    #endregion

    #region Default Methods

    /// <summary>
    /// Verifica se existe uma transação ativa no contexto atual.
    /// </summary>
    /// <returns><c>true</c> se houver uma transação ativa; caso contrário, <c>false</c>.</returns>
    bool HasActiveTransaction();

    /// <summary>
    /// Salva todas as mudanças feitas no contexto de forma assíncrona.
    /// Esta operação também garante que quaisquer eventos de domínio sejam publicados após a persistência bem-sucedida.
    /// </summary>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    Task SaveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva todas as mudanças feitas no contexto para o banco de dados de forma assíncrona
    /// Também garante que quaisquer domain events sejam publicados após a persistência bem-sucedida.
    /// </summary>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém o número de registros gravados no banco de dados.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Inicia uma nova transação de banco de dados de forma assíncrona.
    /// Esta transação encapsula uma série de operações que podem ser confirmadas ou revertidas como uma unidade.
    /// </summary>
    /// <param name="cancellationToken">Token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa representando a operação assíncrona.</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirma a transação atual de forma assíncrona, persistindo todas as alterações realizadas.
    /// </summary>
    /// <param name="cancellationToken">Token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa representando a operação assíncrona.</returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Reverte todas as operações realizadas na transação atual de forma assíncrona.
    /// Esta operação é usada para cancelar todas as mudanças não confirmadas no contexto persistente.
    /// </summary>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken);
    
    #endregion
}