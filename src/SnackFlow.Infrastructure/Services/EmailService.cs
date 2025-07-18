using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.Extensions;
using SnackFlow.Domain.Enums;

namespace SnackFlow.Infrastructure.Services;

public sealed class EmailService(
    IAmazonSimpleEmailService amazonSimpleEmailService,
    ILogger<EmailService> logger)
    : IEmailService
{
    public async Task SendAsync(string to, string subject, string body, EEmailAddress emailAddress)
    {
        try
        {
            var request = new SendEmailRequest
            {
                Source = emailAddress.GetDescription(),
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
            logger.LogError(ex, "exception occurred: {exType} - {Message}", ex.GetType().Name, ex.Message);
            throw;
        }
    }
}