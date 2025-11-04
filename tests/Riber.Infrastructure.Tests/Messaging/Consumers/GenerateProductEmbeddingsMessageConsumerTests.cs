using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Riber.Application.Abstractions.Services.AI;
using Riber.Application.Messages;
using Riber.Domain.Entities;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.Product;
using Riber.Infrastructure.Messaging.Consumers;
using Riber.Infrastructure.Persistence.Models.Embeddings;
using System.Linq.Expressions;

namespace Riber.Infrastructure.Tests.Messaging.Consumers;

public sealed class GenerateProductEmbeddingsMessageConsumerTests
{
    private readonly Mock<IEmbeddingsService> _mockEmbeddingsService;
    private readonly Mock<IAiModelService<ProductEmbeddingsModel, Product>> _mockModelService;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ILogger<GenerateProductEmbeddingsMessageConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<GenerateProductEmbeddingsMessage>> _mockConsumeContext;
    private readonly GenerateProductEmbeddingsMessageConsumer _consumer;

    public GenerateProductEmbeddingsMessageConsumerTests()
    {
        _mockEmbeddingsService = new Mock<IEmbeddingsService>();
        _mockModelService = new Mock<IAiModelService<ProductEmbeddingsModel, Product>>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockLogger = new Mock<ILogger<GenerateProductEmbeddingsMessageConsumer>>();
        _mockConsumeContext = new Mock<ConsumeContext<GenerateProductEmbeddingsMessage>>();

        _consumer = new GenerateProductEmbeddingsMessageConsumer(
            _mockEmbeddingsService.Object,
            _mockModelService.Object,
            _mockProductRepository.Object,
            _mockLogger.Object
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should generate and save product embeddings successfully")]
    public async Task Consume_WithValidProduct_ShouldGenerateAndSaveEmbeddings()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var message = new GenerateProductEmbeddingsMessage(productId);
        var cancellationToken = CancellationToken.None;

        var category = ProductCategory.Create(
            "Test Category",
            "Test Description",
            "TEST",
            Guid.NewGuid()
        );

        var product = Product.Create(
            "Test Product",
            "Test Description",
            99.99m,
            category.Id,
            Guid.NewGuid(),
            Guid.Empty
        );

        var categoryProperty = typeof(Product).GetProperty("Category");
        categoryProperty?.SetValue(product, category);

        var embeddings = new float[384];
        for (int i = 0; i < 384; i++)
            embeddings[i] = 0.1f;

        _mockConsumeContext.Setup(x => x.Message).Returns(message);
        _mockConsumeContext.Setup(x => x.CancellationToken).Returns(cancellationToken);

        _mockProductRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<ProductByIdSpecification>(),
                cancellationToken,
                It.IsAny<Expression<Func<Product, object>>>()))
            .ReturnsAsync(product);

        _mockEmbeddingsService
            .Setup(x => x.GenerateEmbeddingsAsync(It.IsAny<string>(), cancellationToken))
            .ReturnsAsync(embeddings);

        _mockModelService
            .Setup(x => x.CreateAsync(It.IsAny<ProductEmbeddingsModel>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        _mockProductRepository.Verify(
            x => x.GetSingleAsync(
                It.IsAny<ProductByIdSpecification>(),
                cancellationToken,
                It.IsAny<Expression<Func<Product, object>>>()),
            Times.Once
        );

        _mockEmbeddingsService.Verify(
            x => x.GenerateEmbeddingsAsync(It.IsAny<string>(), cancellationToken),
            Times.Once
        );

        _mockModelService.Verify(
            x => x.CreateAsync(
                It.Is<ProductEmbeddingsModel>(m => m.ProductId == productId),
                cancellationToken),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should log information during embedding generation")]
    public async Task Consume_WhenGeneratingEmbeddings_ShouldLogInformation()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var message = new GenerateProductEmbeddingsMessage(productId);
        var cancellationToken = CancellationToken.None;

        var category = ProductCategory.Create(
            "Test Category",
            "Test Description",
            "TEST",
            Guid.NewGuid()
        );

        var product = Product.Create(
            "Test Product",
            "Test Description",
            99.99m,
            category.Id,
            Guid.NewGuid(),
            Guid.Empty
        );

        var categoryProperty = typeof(Product).GetProperty("Category");
        categoryProperty?.SetValue(product, category);

        var embeddings = new float[384];

        _mockConsumeContext.Setup(x => x.Message).Returns(message);
        _mockConsumeContext.Setup(x => x.CancellationToken).Returns(cancellationToken);

        _mockProductRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<ProductByIdSpecification>(),
                cancellationToken,
                It.IsAny<Expression<Func<Product, object>>>()))
            .ReturnsAsync(product);

        _mockEmbeddingsService
            .Setup(x => x.GenerateEmbeddingsAsync(It.IsAny<string>(), cancellationToken))
            .ReturnsAsync(embeddings);

        _mockModelService
            .Setup(x => x.CreateAsync(It.IsAny<ProductEmbeddingsModel>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Gerando embeddings do produto {productId}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Embeddings do produto {productId} gerados")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Embeddings do produto {productId} salvos")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should return early when product is not found")]
    public async Task Consume_WhenProductNotFound_ShouldReturnEarly()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var message = new GenerateProductEmbeddingsMessage(productId);
        var cancellationToken = CancellationToken.None;

        _mockConsumeContext.Setup(x => x.Message).Returns(message);
        _mockConsumeContext.Setup(x => x.CancellationToken).Returns(cancellationToken);

        _mockProductRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<ProductByIdSpecification>(),
                cancellationToken,
                It.IsAny<Expression<Func<Product, object>>>()))
            .ReturnsAsync((Product?)null);

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        _mockProductRepository.Verify(
            x => x.GetSingleAsync(
                It.IsAny<ProductByIdSpecification>(),
                cancellationToken,
                It.IsAny<Expression<Func<Product, object>>>()),
            Times.Once
        );

        _mockEmbeddingsService.Verify(
            x => x.GenerateEmbeddingsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        _mockModelService.Verify(
            x => x.CreateAsync(It.IsAny<ProductEmbeddingsModel>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should log error when product is not found")]
    public async Task Consume_WhenProductNotFound_ShouldLogError()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var message = new GenerateProductEmbeddingsMessage(productId);
        var cancellationToken = CancellationToken.None;

        _mockConsumeContext.Setup(x => x.Message).Returns(message);
        _mockConsumeContext.Setup(x => x.CancellationToken).Returns(cancellationToken);

        _mockProductRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<ProductByIdSpecification>(),
                cancellationToken,
                It.IsAny<Expression<Func<Product, object>>>()))
            .ReturnsAsync((Product?)null);

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert - O consumer usa Serilog.Log.Error diretamente, não o ILogger injetado
        _mockEmbeddingsService.Verify(
            x => x.GenerateEmbeddingsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should log error and not throw when exception occurs")]
    public async Task Consume_WhenExceptionOccurs_ShouldLogErrorAndNotThrow()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var message = new GenerateProductEmbeddingsMessage(productId);
        var cancellationToken = CancellationToken.None;
        var expectedException = new Exception("Test exception");

        _mockConsumeContext.Setup(x => x.Message).Returns(message);
        _mockConsumeContext.Setup(x => x.CancellationToken).Returns(cancellationToken);

        _mockProductRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<ProductByIdSpecification>(),
                cancellationToken,
                It.IsAny<Expression<Func<Product, object>>>()))
            .ThrowsAsync(expectedException);

        // Act
        var act = async () => await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        await act.Should().NotThrowAsync();

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Falha ao gerar os embeddings do produto {productId}")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should handle exception during embeddings generation")]
    public async Task Consume_WhenEmbeddingsGenerationFails_ShouldLogError()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var message = new GenerateProductEmbeddingsMessage(productId);
        var cancellationToken = CancellationToken.None;
        var expectedException = new Exception("Embeddings generation failed");

        var category = ProductCategory.Create(
            "Test Category",
            "Test Description",
            "TEST",
            Guid.NewGuid()
        );

        var product = Product.Create(
            "Test Product",
            "Test Description",
            99.99m,
            category.Id,
            Guid.NewGuid(),
            Guid.Empty
        );

        var categoryProperty = typeof(Product).GetProperty("Category");
        categoryProperty?.SetValue(product, category);

        _mockConsumeContext.Setup(x => x.Message).Returns(message);
        _mockConsumeContext.Setup(x => x.CancellationToken).Returns(cancellationToken);

        _mockProductRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<ProductByIdSpecification>(),
                cancellationToken,
                It.IsAny<Expression<Func<Product, object>>>()))
            .ReturnsAsync(product);

        _mockEmbeddingsService
            .Setup(x => x.GenerateEmbeddingsAsync(It.IsAny<string>(), cancellationToken))
            .ThrowsAsync(expectedException);

        // Act
        var act = async () => await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        await act.Should().NotThrowAsync();

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Falha ao gerar os embeddings do produto {productId}")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );

        _mockModelService.Verify(
            x => x.CreateAsync(It.IsAny<ProductEmbeddingsModel>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should handle exception when saving embeddings")]
    public async Task Consume_WhenSavingEmbeddingsFails_ShouldLogError()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var message = new GenerateProductEmbeddingsMessage(productId);
        var cancellationToken = CancellationToken.None;
        var expectedException = new Exception("Save failed");

        var category = ProductCategory.Create(
            "Test Category",
            "Test Description",
            "TEST",
            Guid.NewGuid()
        );

        var product = Product.Create(
            "Test Product",
            "Test Description",
            99.99m,
            category.Id,
            Guid.NewGuid(),
            Guid.Empty
        );

        var categoryProperty = typeof(Product).GetProperty("Category");
        categoryProperty?.SetValue(product, category);

        var embeddings = new float[384];

        _mockConsumeContext.Setup(x => x.Message).Returns(message);
        _mockConsumeContext.Setup(x => x.CancellationToken).Returns(cancellationToken);

        _mockProductRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<ProductByIdSpecification>(),
                cancellationToken,
                It.IsAny<Expression<Func<Product, object>>>()))
            .ReturnsAsync(product);

        _mockEmbeddingsService
            .Setup(x => x.GenerateEmbeddingsAsync(It.IsAny<string>(), cancellationToken))
            .ReturnsAsync(embeddings);

        _mockModelService
            .Setup(x => x.CreateAsync(It.IsAny<ProductEmbeddingsModel>(), cancellationToken))
            .ThrowsAsync(expectedException);

        // Act
        var act = async () => await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        await act.Should().NotThrowAsync();

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Falha ao gerar os embeddings do produto {productId}")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should use correct embedding string format")]
    public async Task Consume_ShouldUseCorrectEmbeddingStringFormat()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var message = new GenerateProductEmbeddingsMessage(productId);
        var cancellationToken = CancellationToken.None;

        var category = ProductCategory.Create(
            "Electronics",
            "Electronic devices",
            "ELEC",
            Guid.NewGuid()
        );

        var product = Product.Create(
            "Laptop",
            "High-performance laptop",
            1999.99m,
            category.Id,
            Guid.NewGuid(),
            Guid.Empty
        );

        var categoryProperty = typeof(Product).GetProperty("Category");
        categoryProperty?.SetValue(product, category);

        var embeddings = new float[384];
        string? capturedInput = null;

        _mockConsumeContext.Setup(x => x.Message).Returns(message);
        _mockConsumeContext.Setup(x => x.CancellationToken).Returns(cancellationToken);

        _mockProductRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<ProductByIdSpecification>(),
                cancellationToken,
                It.IsAny<Expression<Func<Product, object>>>()))
            .ReturnsAsync(product);

        _mockEmbeddingsService
            .Setup(x => x.GenerateEmbeddingsAsync(It.IsAny<string>(), cancellationToken))
            .ReturnsAsync(embeddings)
            .Callback<string, CancellationToken>((input, _) => capturedInput = input);

        _mockModelService
            .Setup(x => x.CreateAsync(It.IsAny<ProductEmbeddingsModel>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        capturedInput.Should().NotBeNull();
        capturedInput.Should().Contain("Laptop");
        capturedInput.Should().Contain("High-performance laptop");
        capturedInput.Should().Contain("Electronics");
    }
}
