using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Services.Email;

namespace Riber.Infrastructure.Services.AWS.Email;

public sealed class AmazonSESService(
    IAmazonSimpleEmailService amazonSimpleEmailService,
    ILogger<AmazonSESService> logger)
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
            logger.LogError(ex,
                "[{ClassName}] exceção inesperada: {ExceptionType} - {ExceptionMessage}",
                nameof(AmazonSESService),
                ex.GetType(),
                ex.Message);
            throw;
        }
    }
}