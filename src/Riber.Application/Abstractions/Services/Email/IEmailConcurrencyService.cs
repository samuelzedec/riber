namespace Riber.Application.Abstractions.Services.Email;

/// <summary>
/// Fornece funcionalidade para gerenciar concorrência durante operações relacionadas a emails.
/// </summary>
public interface IEmailConcurrencyService
{
    /// <summary>
    /// Obtém um bloqueio de concorrência para gerenciar o acesso durante operações relacionadas a e-mails.
    /// </summary>
    /// <param name="cancellationToken">
    /// Um token para monitorar solicitações de cancelamento.
    /// </param>
    /// <returns>
    /// Uma tarefa que representa uma operação assíncrona. O resultado contém um objeto descartável
    /// que deve ser descartado para liberar o bloqueio.
    /// </returns>
    Task<IDisposable> AcquireAsync(CancellationToken cancellationToken = default);
}