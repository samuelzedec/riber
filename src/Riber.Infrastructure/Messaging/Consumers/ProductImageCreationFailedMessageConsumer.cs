using MassTransit;
using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Services;
using Riber.Application.Messages;

namespace Riber.Infrastructure.Messaging.Consumers;

public sealed class ProductImageCreationFailedMessageConsumer(
    IImageStorageService imageStorageService,
    ILogger<ProductImageCreationFailedMessageConsumer> logger)
    : IConsumer<ProductImageCreationFailedMessage>
{
    public async Task Consume(ConsumeContext<ProductImageCreationFailedMessage> context)
    {
        var message = context.Message;
        try
        {
            if (string.IsNullOrWhiteSpace(message.ImageKey))
            {
                logger.LogWarning("Image key is empty. Job will be retried");
                return;
            }

            logger.LogInformation("Starting deletion of image {ImageKey} from storage", message.ImageKey);
            await imageStorageService.DeleteAllAsync(message.ImageKey);
            logger.LogInformation("Successfully deleted image {ImageKey} from storage", message.ImageKey);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete image {ImageKey} from storage. Job will be retried",
                message.ImageKey);
        }
    }
}