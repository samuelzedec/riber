using MassTransit;
using Riber.Application.Abstractions.Messaging;

namespace Riber.Infrastructure.Messaging;

public sealed class MassTransitPublishContextWrapper(PublishContext context) 
    : IPublishContext
{
    public void SetPriority(byte priority)
        => context.SetPriority(priority);

    public void SetDelay(TimeSpan delay)
        => context.Delay = delay;

    public void SetHeader(string key, object value)
        => context.Headers.Set(key, value);
}