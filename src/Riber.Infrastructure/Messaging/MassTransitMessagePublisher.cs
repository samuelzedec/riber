using MassTransit;
using Riber.Application.Abstractions.Messaging;

namespace Riber.Infrastructure.Messaging;

public sealed class MassTransitMessagePublisher(IPublishEndpoint publishEndpoint)
    : IMessagePublisher
{
    public async Task PublishAsync<TMessage>(
        TMessage message,
        CancellationToken cancellationToken = default)
        where TMessage : class
        => await publishEndpoint.Publish(message, cancellationToken);

    public async Task PublishAsync<TMessage>(
        TMessage message,
        Action<IPublishContext>? configure,
        CancellationToken cancellationToken = default)
        where TMessage : class
        => await publishEndpoint.Publish(message, context =>
        {
            var wrapper = new MassTransitPublishContextWrapper(context);
            configure?.Invoke(wrapper);
        }, cancellationToken);
}