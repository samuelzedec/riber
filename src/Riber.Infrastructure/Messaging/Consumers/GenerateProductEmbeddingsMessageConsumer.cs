using MassTransit;
using Microsoft.Extensions.Logging;
using Pgvector;
using Riber.Application.Abstractions.Services.AI;
using Riber.Application.Messages;
using Riber.Domain.Entities.Catalog;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.Product;
using Riber.Infrastructure.Persistence.Models.Embeddings;
using Serilog;

namespace Riber.Infrastructure.Messaging.Consumers;

public sealed class GenerateProductEmbeddingsMessageConsumer(
    IEmbeddingsService embeddingsService,
    IAiModelService<ProductEmbeddingsModel, Product> modelService,
    IProductRepository productRepository,
    ILogger<GenerateProductEmbeddingsMessageConsumer> logger)
    : IConsumer<GenerateProductEmbeddingsMessage>
{
    public async Task Consume(ConsumeContext<GenerateProductEmbeddingsMessage> context)
    {
        var productId = context.Message.ProductId;
        try
        {
            logger.LogInformation("Gerando embeddings do produto {ProductId}.", productId);

            var product = await productRepository
                .GetSingleAsync(new ProductByIdSpecification(productId), includes: p => p.Category);

            if (product is null)
            {
                Log.Error("Produto com ID {ProductId} não encontrado.", productId);
                return;
            }

            var embeddings = await embeddingsService.GenerateEmbeddingsAsync(
                ProductEmbeddingsModel.ToEmbeddingString(product));

            logger.LogInformation("Embeddings do produto {ProductId} gerados.", productId);
            var productEmbeddingsModel = new ProductEmbeddingsModel
            {
                ProductId = productId, 
                Embeddings = new Vector(embeddings)
            };

            await modelService.CreateAsync(productEmbeddingsModel, context.CancellationToken);
            logger.LogInformation("Embeddings do produto {ProductId} salvos.", productId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao gerar os embeddings do produto {ProductId}.", productId);
        }
    }
}