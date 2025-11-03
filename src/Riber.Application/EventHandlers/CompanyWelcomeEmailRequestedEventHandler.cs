using Riber.Application.Abstractions.Events;
using Riber.Application.Abstractions.Messaging;
using Riber.Application.Extensions;
using Riber.Application.Messages;
using Riber.Domain.Enums;
using @event = Riber.Domain.Events.CompanyWelcomeEmailRequestedEvent;

namespace Riber.Application.EventHandlers;

internal sealed class CompanyWelcomeEmailRequestedEventHandler(
    IMessagePublisher messagePublisher)
    : IDomainEventHandler<@event>
{
    public async ValueTask Handle(@event notification, CancellationToken cancellationToken)
    {
        var message = new SendEmailMessage(
            TemplatePath: $"{EmailAudience.Company.GetDescription()}-{EmailTemplate.Welcome.GetDescription()}",
            Subject: "Seja bem-vindo ao Riber!",
            To: "samuel.ribeiro77f@gmail.com",
            From: EmailAddress.NoReply.GetDescription(),
            Model: notification.ExtractProperties()
        );
        await messagePublisher.PublishAsync(message, cancellationToken);
    }
}