using Riber.Application.Abstractions.Dispatchers;
using Riber.Application.Abstractions.Events;

namespace Riber.Application.Features.Products.Events;

public sealed class ImageDeletedFromStorageEventHandler(
    IDeleteImageFromStorageDispatcher dispatcher)
    : IApplicationEventHandler<ImageDeletedFromStorageEvent>
{
    public async ValueTask Handle(ImageDeletedFromStorageEvent notification, CancellationToken cancellationToken)
        => await dispatcher.SendAsync(notification.ImageKey, cancellationToken);
}