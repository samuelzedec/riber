using Riber.Application.Abstractions.Events;
using Riber.Application.Abstractions.Messaging;
using Riber.Application.Messages;
using Riber.Domain.Events;

namespace Riber.Application.EventHandlers;

public sealed class GenerateProductEmbeddingsEventHandler(
    IMessagePublisher messagePublisher)
    : IDomainEventHandler<GenerateProductEmbeddingsEvent>
{
    public async ValueTask Handle(GenerateProductEmbeddingsEvent notification, CancellationToken cancellationToken)
    {
        var message = new GenerateProductEmbeddingsMessage(notification.ProductId);
        await messagePublisher.PublishAsync(message, cancellationToken);
    }
}