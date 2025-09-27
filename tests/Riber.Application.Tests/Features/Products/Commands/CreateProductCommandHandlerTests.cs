using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Riber.Application.Abstractions.Services;
using Riber.Application.Features.Products.Commands;
using Riber.Domain.Entities;
using Riber.Domain.Repositories;
using Riber.Domain.Tests;

namespace Riber.Application.Tests.Features.Products.Commands;

public sealed class CreateProductCommandHandlerTests : BaseTest
{
    #region Setup

    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IImageStorageService> _mockImageStorageService;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<ILogger<CreateProductCommandHandler>> _mockLogger;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly CreateProductCommandHandler _handler;
    private readonly CreateProductCommand _command;
    private readonly Guid _companyId;

    public CreateProductCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockImageStorageService = new Mock<IImageStorageService>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockLogger = new Mock<ILogger<CreateProductCommandHandler>>();
        _mockProductRepository = new Mock<IProductRepository>();
        _companyId = Guid.NewGuid();

        _mockUnitOfWork.Setup(x => x.Products).Returns(_mockProductRepository.Object);
        _mockCurrentUserService.Setup(x => x.GetCompanyId()).Returns(_companyId);

        _handler = new CreateProductCommandHandler(
            _mockUnitOfWork.Object,
            _mockImageStorageService.Object,
            _mockCurrentUserService.Object,
            _mockLogger.Object
        );

        _command = new CreateProductCommand(
            Name: _faker.Commerce.ProductName(),
            Description: _faker.Commerce.ProductDescription(),
            Price: _faker.Random.Decimal(10, 1000),
            CategoryId: Guid.NewGuid(),
            ImageStream: null,
            ImageName: string.Empty,
            ImageContent: string.Empty
        );
    }

    #endregion

    #region Success Tests

    [Fact(DisplayName = "Creating product without image should return success response")]
    public async Task Handle_WhenValidDataWithoutImage_ShouldReturnSuccessResponse()
    {
        // Arrange
        _mockProductRepository
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(_command.Name);
        result.Value.Description.Should().Be(_command.Description);
        result.Value.Price.Should().Be(_command.Price);
        result.Value.ImageName.Should().Be(string.Empty);
        result.Value.ProductId.Should().NotBeEmpty();

        _mockProductRepository.Verify(x => x.CreateAsync(
            It.Is<Product>(p => 
                p.Name == _command.Name &&
                p.Description == _command.Description &&
                p.UnitPrice == _command.Price &&
                p.CategoryId == _command.CategoryId &&
                p.CompanyId == _companyId &&
                p.ImageUrl == null),
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockCurrentUserService.Verify(x => x.GetCompanyId(), Times.Once);

        _mockImageStorageService.Verify(x => x.UploadAsync(
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Creating product with image should upload image and return success response")]
    public async Task Handle_WhenValidDataWithImage_ShouldUploadImageAndReturnSuccessResponse()
    {
        // Arrange
        var imageStream = new MemoryStream();
        var imageName = "test-image.jpg";
        var imageContent = "image/jpeg";
        var expectedImageUrl = "https://storage.example.com/test-image.jpg";

        var commandWithImage = _command with
        {
            ImageStream = imageStream,
            ImageName = imageName,
            ImageContent = imageContent
        };

        _mockImageStorageService
            .Setup(x => x.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(expectedImageUrl);

        _mockProductRepository
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(commandWithImage, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(commandWithImage.Name);
        result.Value.Description.Should().Be(commandWithImage.Description);
        result.Value.Price.Should().Be(commandWithImage.Price);
        result.Value.ImageName.Should().Be(expectedImageUrl);
        result.Value.ProductId.Should().NotBeEmpty();

        _mockImageStorageService.Verify(x => x.UploadAsync(
            imageStream,
            imageName,
            imageContent), Times.Once);

        _mockProductRepository.Verify(x => x.CreateAsync(
            It.Is<Product>(p => 
                p.Name == commandWithImage.Name &&
                p.Description == commandWithImage.Description &&
                p.UnitPrice == commandWithImage.Price &&
                p.CategoryId == commandWithImage.CategoryId &&
                p.CompanyId == _companyId &&
                p.ImageUrl == expectedImageUrl),
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockCurrentUserService.Verify(x => x.GetCompanyId(), Times.Once);
    }

    #endregion

    #region Exception Tests

    [Fact(DisplayName = "Creating product when image upload fails should log error and rethrow")]
    public async Task Handle_WhenImageUploadFails_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var imageStream = new MemoryStream();
        var imageName = "test-image.jpg";
        var imageContent = "image/jpeg";
        var expectedException = new InvalidOperationException("Image upload failed");

        var commandWithImage = _command with
        {
            ImageStream = imageStream,
            ImageName = imageName,
            ImageContent = imageContent
        };

        _mockImageStorageService
            .Setup(x => x.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ThrowsAsync(expectedException);

        // Act
        var result = async () => await _handler.Handle(commandWithImage, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Image upload failed");

        _mockImageStorageService.Verify(x => x.UploadAsync(
            imageStream, imageName, imageContent), Times.Once);

        _mockProductRepository.Verify(x => x.CreateAsync(
            It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "Creating product when product creation fails should log error and rethrow")]
    public async Task Handle_WhenProductCreationFails_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Product creation failed");

        _mockProductRepository
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Product creation failed");

        _mockProductRepository.Verify(x => x.CreateAsync(
            It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);

        _mockCurrentUserService.Verify(x => x.GetCompanyId(), Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "Creating product when save changes fails should log error and rethrow")]
    public async Task Handle_WhenSaveChangesFails_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Save changes failed");

        _mockProductRepository
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Save changes failed");

        _mockProductRepository.Verify(x => x.CreateAsync(
            It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockCurrentUserService.Verify(x => x.GetCompanyId(), Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "Creating product when current user service fails should log error and rethrow")]
    public async Task Handle_WhenCurrentUserServiceFails_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var expectedException = new UnauthorizedAccessException("User not authenticated");

        _mockCurrentUserService
            .Setup(x => x.GetCompanyId())
            .Throws(expectedException);

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<UnauthorizedAccessException>()
            .WithMessage("User not authenticated");

        _mockCurrentUserService.Verify(x => x.GetCompanyId(), Times.Once);

        _mockProductRepository.Verify(x => x.CreateAsync(
            It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region Cancellation Tests

    [Fact(DisplayName = "Should respect cancellation token during image upload")]
    public async Task Handle_WhenCancellationTokenDuringImageUpload_ShouldRespectCancellationToken()
    {
        // Arrange
        var imageStream = new MemoryStream();
        var imageName = "test-image.jpg";
        var imageContent = "image/jpeg";

        var commandWithImage = _command with
        {
            ImageStream = imageStream,
            ImageName = imageName,
            ImageContent = imageContent
        };

        _mockImageStorageService
            .Setup(x => x.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handler.Handle(commandWithImage, CancellationToken.None);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockImageStorageService.Verify(x => x.UploadAsync(
            imageStream, imageName, imageContent), Times.Once);

        _mockProductRepository.Verify(x => x.CreateAsync(
            It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Should respect cancellation token during product creation")]
    public async Task Handle_WhenCancellationTokenDuringProductCreation_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockCancellationToken = new CancellationToken(true);

        _mockProductRepository
            .Setup(x => x.CreateAsync(
                It.IsAny<Product>(),
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handler.Handle(_command, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockProductRepository.Verify(x => x.CreateAsync(
            It.IsAny<Product>(), 
            It.Is<CancellationToken>(ct => ct.IsCancellationRequested)), Times.Once);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);

        _mockCurrentUserService.Verify(x => x.GetCompanyId(), Times.Once);
    }

    [Fact(DisplayName = "Should respect cancellation token during save changes")]
    public async Task Handle_WhenCancellationTokenDuringSaveChanges_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockCancellationToken = new CancellationToken(true);

        _mockProductRepository
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handler.Handle(_command, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockProductRepository.Verify(x => x.CreateAsync(
            It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.Is<CancellationToken>(ct => ct.IsCancellationRequested)), Times.Once);

        _mockCurrentUserService.Verify(x => x.GetCompanyId(), Times.Once);
    }

    #endregion
}