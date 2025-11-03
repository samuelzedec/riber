using Riber.Application.Abstractions.Events;
using Riber.Application.Abstractions.Messaging;
using Riber.Application.Messages;
using Riber.Domain.Events;

namespace Riber.Application.EventHandlers;

public sealed class ProductImageCreationFailedEventHandler(
    IMessagePublisher messagePublisher)
    : IDomainEventHandler<ProductImageCreationFailedEvent>
{
    public async ValueTask Handle(ProductImageCreationFailedEvent notification, CancellationToken cancellationToken)
    {
        var message = new ProductImageCreationFailedMessage(notification.ImageKey);
        await messagePublisher.PublishAsync(message, cancellationToken);
    }
}