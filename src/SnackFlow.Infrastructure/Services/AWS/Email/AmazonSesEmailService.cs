using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using SnackFlow.Application.Abstractions.Services.Email;
using SnackFlow.Domain.Constants;

namespace SnackFlow.Infrastructure.Services.AWS.Email;

public sealed class AmazonSesEmailService(
    IAmazonSimpleEmailService amazonSimpleEmailService,
    ILogger<AmazonSesEmailService> logger)
    : IEmailService
{
    public async Task SendAsync(string to, string subject, string body, string emailAddress)
    {
        try
        {
            var request = new SendEmailRequest
            {
                Source = emailAddress,
                Destination = new Destination { ToAddresses = [to] },
                Message = new Message
                {
                    Subject = new Content { Data = subject },
                    Body = new Body { Html = new Content { Data = body } }
                }
            };
            
            await amazonSimpleEmailService.SendEmailAsync(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessage.Exception.Unexpected(ex.GetType().Name, ex.Message));
            throw;
        }
    }
}