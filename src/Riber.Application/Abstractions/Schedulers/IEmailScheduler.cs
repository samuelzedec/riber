using Riber.Domain.Enums;

namespace Riber.Application.Abstractions.Schedulers;

/// <summary>
/// Define um contrato para agendar tarefas de email dentro da aplicação.
/// </summary>
public interface IEmailScheduler
{
    /// <summary>
    /// Define um agendamento de email para ser enviado de forma assíncrona com base nos dados fornecidos.
    /// </summary>
    /// <param name="emailAddress">O endereço de email do remetente a ser utilizado no envio.</param>
    /// <param name="emailData">Um objeto contendo as informações necessárias para compor e agendar o email.</param>
    /// <param name="cancellationToken">Um token para monitorar solicitações de cancelamento, permitindo cancelar a operação se necessário.</param>
    /// <returns>Uma tarefa representando a operação assíncrona.</returns>
    Task ScheduleEmailAsync(EmailAddress emailAddress, object emailData, CancellationToken cancellationToken = default);
}