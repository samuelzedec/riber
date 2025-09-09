namespace SnackFlow.Application.Abstractions.Services.Email;

/// <summary>
/// Define um serviço para envio de emails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envia um e-mail de forma assíncrona.
    /// </summary>
    /// <param name="to">O endereço de e-mail do destinatário.</param>
    /// <param name="subject">O assunto do e-mail.</param>
    /// <param name="body">O conteúdo do corpo do e-mail.</param>
    /// <param name="emailAddress">O endereço de e-mail do remetente como uma string.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona de envio do e-mail.</returns>
    Task SendAsync(string to, string subject, string body, string emailAddress);
}