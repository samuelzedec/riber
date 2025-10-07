using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Riber.Application.Abstractions.Services.Email;

namespace Riber.Infrastructure.Services.AWS.Email;

public sealed class AmazonSESService(
    IAmazonSimpleEmailService amazonSimpleEmailService)
    : IEmailService
{
    public async Task SendAsync(string to, string subject, string body, string emailAddress)
    {
        var request = new SendEmailRequest
        {
            Source = emailAddress,
            Destination = new Destination { ToAddresses = [to] },
            Message = new Message
            {
                Subject = new Content { Data = subject }, Body = new Body { Html = new Content { Data = body } }
            }
        };

        await amazonSimpleEmailService.SendEmailAsync(request);
    }
}