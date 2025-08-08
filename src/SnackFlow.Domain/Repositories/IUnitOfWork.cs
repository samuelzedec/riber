namespace SnackFlow.Domain.Repositories;

/// <summary>
/// Representa uma unidade de trabalho que encapsula um conjunto de mudanças para persistência.
/// Fornece mecanismos para gerenciar transações e repositórios de forma coesa.
/// </summary>
public interface IUnitOfWork
{
    #region Repositories
    
    ICompanyRepository Companies { get; }
    
    IUserRepository Users { get; }
    
    IInvitationRepository Invitations { get; }
    
    #endregion

    #region Default Methods

    /// <summary>
    /// Salva todas as mudanças feitas no contexto de forma assíncrona.
    /// Esta operação também garante que quaisquer eventos de domínio sejam publicados após a persistência bem-sucedida.
    /// </summary>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    Task SaveAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Salva todas as mudanças feitas no contexto para o banco de dados de forma assíncrona
    /// Também garante que quaisquer domain events sejam publicados após a persistência bem-sucedida.
    /// </summary>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém o número de registros gravados no banco de dados.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Inicia uma nova transação de banco de dados de forma assíncrona.
    /// Esta transação encapsula uma série de operações de banco de dados que podem ser confirmadas ou revertidas como uma unidade.
    /// </summary>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Confirma a transação atual de forma assíncrona, garantindo a persistência de todas as alterações realizadas durante a transação.
    /// </summary>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação de confirmação assíncrona.</returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Reverte todas as operações realizadas na transação atual de forma assíncrona.
    /// Esta operação é usada para cancelar todas as mudanças não confirmadas no contexto persistente.
    /// </summary>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Libera os recursos alocados pela instância de forma assíncrona.
    /// Este método garante que quaisquer transações ou contextos associados sejam devidamente descartados.
    /// </summary>
    /// <returns>Uma tarefa que representa a operação de descarte assíncrono.</returns>
    ValueTask DisposeAsync();

    /// <summary>
    /// Libera os recursos gerenciados e não gerenciados utilizados pela unidade de trabalho.
    /// Esta operação é responsável por encerrar conexões, transações ou qualquer outro recurso alocado
    /// durante o ciclo de vida do contexto ou repositório associado.
    /// </summary>
    void Dispose();

    #endregion
}