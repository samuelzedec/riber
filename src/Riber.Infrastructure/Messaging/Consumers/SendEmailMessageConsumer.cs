using MassTransit;
using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Services.Email;
using Riber.Application.Messages;

namespace Riber.Infrastructure.Messaging.Consumers;

public sealed class SendEmailMessageConsumer(
    IEmailService emailService,
    IEmailTemplateRender templateEmailRender,
    IEmailConcurrencyService emailConcurrencyService,
    ILogger<SendEmailMessageConsumer> logger)
    : IConsumer<SendEmailMessage>
{
    public async Task Consume(ConsumeContext<SendEmailMessage> context)
    {
        var message = context.Message;
        try
        {
            using var _ = await emailConcurrencyService.AcquireAsync(context.CancellationToken);
            logger.LogInformation("Sending email to {To}", message.To);

            var htmlContent = await templateEmailRender
                .GetTemplateAsync(message.TemplatePath, message.Model);

            await emailService.SendAsync(
                message.To,
                message.Subject,
                htmlContent,
                message.From
            );

            logger.LogInformation("Successfully sent email to {To}", message.To);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Permanent failure sending email to {To}. Will NOT retry.", message.To);
        }
    }
}