using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Riber.Application.Abstractions.Services.Email;

namespace Riber.Infrastructure.Services.AWS.Email;

public sealed class AmazonSESService(
    IAmazonSimpleEmailService amazonSimpleEmailService)
    : IEmailService
{
    public async Task SendAsync(string to, string subject, string htmlContent, string from)
    {
        var request = new SendEmailRequest
        {
            Source = from,
            Destination = new Destination { ToAddresses = [to] },
            Message = new Message
            {
                Subject = new Content { Data = subject },
                Body = new Body { Html = new Content { Data = htmlContent } }
            }
        };

        await amazonSimpleEmailService.SendEmailAsync(request);
    }
}