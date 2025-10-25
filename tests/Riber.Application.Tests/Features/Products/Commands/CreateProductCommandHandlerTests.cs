using System.Net;
using FluentAssertions;
using Mediator;
using Moq;
using Riber.Application.Abstractions.Services;
using Riber.Application.Exceptions;
using Riber.Application.Features.Products.Commands;
using Riber.Application.Features.Products.Events;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Entities;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.ProductCategory;
using Riber.Domain.Tests;

namespace Riber.Application.Tests.Features.Products.Commands;

public sealed class CreateProductCommandHandlerTests : BaseTest
{
    #region Setup

    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IImageStorageService> _mockImageStorageService;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly CreateProductCommandHandler _handler;
    private readonly CreateProductCommand _command;
    private readonly CreateProductCommand _commandWithoutImage;
    private readonly Guid _companyId;
    private readonly ProductCategory _productCategory;

    public CreateProductCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockImageStorageService = new Mock<IImageStorageService>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockMediator = new Mock<IMediator>();
        _mockProductRepository = new Mock<IProductRepository>();

        _companyId = Guid.NewGuid();
        Guid categoryId = Guid.NewGuid();

        _mockUnitOfWork.Setup(x => x.Products).Returns(_mockProductRepository.Object);
        _mockCurrentUserService.Setup(x => x.GetCompanyId()).Returns(_companyId);

        _handler = new CreateProductCommandHandler(
            _mockUnitOfWork.Object,
            _mockImageStorageService.Object,
            _mockCurrentUserService.Object,
            _mockMediator.Object
        );

        Stream imageStream = new MemoryStream([1, 2, 3, 4, 5]);

        _command = new CreateProductCommand(
            Name: _faker.Commerce.ProductName(),
            Description: _faker.Commerce.ProductDescription(),
            Price: decimal.Parse(_faker.Commerce.Price()),
            CategoryId: categoryId,
            ImageStream: imageStream,
            ImageName: _faker.System.FileName("jpg"),
            ImageContent: "image/jpeg"
        );

        _commandWithoutImage = _command with
        {
            ImageStream = null, ImageName = string.Empty, ImageContent = string.Empty
        };

        _productCategory = ProductCategory.Create(
            code: _faker.Commerce.Categories(1).First().ToUpper(),
            name: _faker.Commerce.Department(),
            description: _faker.Lorem.Sentence(),
            companyId: _companyId
        );
    }

    #endregion

    #region Success Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Creating product with valid data and image should return success response")]
    public async Task Handle_WhenValidDataWithImage_ShouldReturnSuccessResponse()
    {
        // Arrange
        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<ProductCategoryIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_productCategory);

        _mockImageStorageService
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()));

        _mockProductRepository
            .Setup(x => x.CreateImageAsync(It.IsAny<Image>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        _mockProductRepository
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.ProductId.Should().NotBeEmpty();
        result.Value.Name.Should().Be(_command.Name);
        result.Value.Description.Should().Be(_command.Description);
        result.Value.Price.Should().Be(_command.Price);
        result.Value.ImageName.Should().NotBeNullOrEmpty();

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.Is<ProductCategoryIdSpecification>(spec => true),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockImageStorageService.Verify(x => x.UploadAsync(
            It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockProductRepository.Verify(x => x.CreateImageAsync(
            It.IsAny<Image>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.CreateAsync(
            It.Is<Product>(p =>
                p.Name == _command.Name &&
                p.Description == _command.Description &&
                p.UnitPrice == _command.Price &&
                p.CategoryId == _command.CategoryId &&
                p.CompanyId == _companyId),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockCurrentUserService.Verify(x => x.GetCompanyId(), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Creating product with valid data without image should return success response")]
    public async Task Handle_WhenValidDataWithoutImage_ShouldReturnSuccessResponse()
    {
        // Arrange
        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<ProductCategoryIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_productCategory);

        _mockProductRepository
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(_commandWithoutImage, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.ProductId.Should().NotBeEmpty();
        result.Value.Name.Should().Be(_commandWithoutImage.Name);
        result.Value.Description.Should().Be(_commandWithoutImage.Description);
        result.Value.Price.Should().Be(_commandWithoutImage.Price);
        result.Value.ImageName.Should().BeEmpty();

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.Is<ProductCategoryIdSpecification>(spec => true),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockImageStorageService.Verify(x => x.UploadAsync(
            It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockProductRepository.Verify(x => x.CreateImageAsync(
            It.IsAny<Image>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockProductRepository.Verify(x => x.CreateAsync(
            It.Is<Product>(p =>
                p.Name == _commandWithoutImage.Name &&
                p.Description == _commandWithoutImage.Description &&
                p.UnitPrice == _commandWithoutImage.Price &&
                p.CategoryId == _commandWithoutImage.CategoryId &&
                p.CompanyId == _companyId &&
                p.ImageId == null),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockCurrentUserService.Verify(x => x.GetCompanyId(), Times.Once);
    }

    #endregion

    #region Not Found Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Creating product when category does not exist should return not found failure")]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldReturnNotFoundFailure()
    {
        // Arrange
        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<ProductCategoryIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductCategory?)null);

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Message.Should().Be(NotFoundErrors.Category);
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.Is<ProductCategoryIdSpecification>(spec => true),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockImageStorageService.Verify(x => x.UploadAsync(
            It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockProductRepository.Verify(x => x.CreateAsync(
            It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Exception Handling Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Creating product when image upload fails should rollback transaction and cleanup image")]
    public async Task Handle_WhenImageUploadFails_ShouldRollbackTransactionAndCleanupImage()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Image upload failed");

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<ProductCategoryIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_productCategory);

        _mockImageStorageService
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(expectedException);

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Image upload failed");

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.Is<ProductCategoryIdSpecification>(spec => true),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockImageStorageService.Verify(x => x.UploadAsync(
            It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockMediator.Verify(x => x.Publish(
            It.IsAny<ImageDeletedFromStorageEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockProductRepository.Verify(x => x.CreateAsync(
            It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Creating product when product creation fails should rollback transaction and cleanup image")]
    public async Task Handle_WhenProductCreationFails_ShouldRollbackTransactionAndCleanupImage()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Product creation failed");

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<ProductCategoryIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_productCategory);

        _mockImageStorageService
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()));

        _mockProductRepository
            .Setup(x => x.CreateImageAsync(It.IsAny<Image>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        _mockProductRepository
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        _mockMediator
            .Setup(x => x.Publish(It.IsAny<ImageDeletedFromStorageEvent>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Product creation failed");

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.Is<ProductCategoryIdSpecification>(spec => true),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockImageStorageService.Verify(x => x.UploadAsync(
            It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockProductRepository.Verify(x => x.CreateImageAsync(
            It.IsAny<Image>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.CreateAsync(
            It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockMediator.Verify(x => x.Publish(
            It.IsAny<ImageDeletedFromStorageEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Creating product when commit transaction fails should rollback transaction and cleanup image")]
    public async Task Handle_WhenCommitTransactionFails_ShouldRollbackTransactionAndCleanupImage()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Commit transaction failed");

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<ProductCategoryIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_productCategory);

        _mockImageStorageService
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()));

        _mockProductRepository
            .Setup(x => x.CreateImageAsync(It.IsAny<Image>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        _mockProductRepository
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        _mockMediator
            .Setup(x => x.Publish(It.IsAny<ImageDeletedFromStorageEvent>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Commit transaction failed");

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.Is<ProductCategoryIdSpecification>(spec => true),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockImageStorageService.Verify(x => x.UploadAsync(
            It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockProductRepository.Verify(x => x.CreateImageAsync(
            It.IsAny<Image>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.CreateAsync(
            It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockMediator.Verify(x => x.Publish(
            It.IsAny<ImageDeletedFromStorageEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Creating product when NotFoundException occurs should rollback transaction without cleanup")]
    public async Task Handle_WhenNotFoundExceptionOccurs_ShouldRollbackTransactionWithoutCleanup()
    {
        // Arrange
        var expectedException = new NotFoundException("Resource not found");

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<ProductCategoryIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_productCategory);

        _mockImageStorageService
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(expectedException);

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage("Resource not found");

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.Is<ProductCategoryIdSpecification>(spec => true),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockImageStorageService.Verify(x => x.UploadAsync(
            It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockMediator.Verify(x => x.Publish(
            It.IsAny<ImageDeletedFromStorageEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Creating product when InternalException occurs should rollback transaction without cleanup")]
    public async Task Handle_WhenInternalExceptionOccurs_ShouldRollbackTransactionWithoutCleanup()
    {
        // Arrange
        var expectedException = new InternalException("Internal error");

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<ProductCategoryIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_productCategory);

        _mockImageStorageService
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(expectedException);

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InternalException>()
            .WithMessage("Internal error");

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.Is<ProductCategoryIdSpecification>(spec => true),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockImageStorageService.Verify(x => x.UploadAsync(
            It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockMediator.Verify(x => x.Publish(
            It.IsAny<ImageDeletedFromStorageEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Cancellation Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should respect cancellation token during begin transaction")]
    public async Task Handle_WhenCancellationTokenDuringBeginTransaction_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockCancellationToken = new CancellationToken(true);

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handler.Handle(_command, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.IsAny<ProductCategoryIdSpecification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should respect cancellation token during category verification")]
    public async Task Handle_WhenCancellationTokenDuringCategoryVerification_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockCancellationToken = new CancellationToken(true);

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<ProductCategoryIdSpecification>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handler.Handle(_command, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.IsAny<ProductCategoryIdSpecification>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockImageStorageService.Verify(x => x.UploadAsync(
            It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should respect cancellation token during image save")]
    public async Task Handle_WhenCancellationTokenDuringImageSave_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockCancellationToken = new CancellationToken(true);

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<ProductCategoryIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_productCategory);

        _mockProductRepository
            .Setup(x => x.CreateImageAsync(It.IsAny<Image>(),
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)))
            .ThrowsAsync(new OperationCanceledException());

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = async () => await _handler.Handle(_command, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.IsAny<ProductCategoryIdSpecification>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.CreateImageAsync(
            It.IsAny<Image>(),
            It.Is<CancellationToken>(ct => ct.IsCancellationRequested)), Times.Once);
        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should respect cancellation token during product creation")]
    public async Task Handle_WhenCancellationTokenDuringProductCreation_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockCancellationToken = new CancellationToken(true);

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<ProductCategoryIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_productCategory);

        _mockImageStorageService
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()));

        _mockProductRepository
            .Setup(x => x.CreateImageAsync(It.IsAny<Image>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        _mockProductRepository
            .Setup(x => x.CreateAsync(It.IsAny<Product>(),
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)))
            .ThrowsAsync(new OperationCanceledException());

        _mockMediator
            .Setup(x => x.Publish(It.IsAny<ImageDeletedFromStorageEvent>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = async () => await _handler.Handle(_command, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.IsAny<ProductCategoryIdSpecification>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.CreateAsync(
            It.IsAny<Product>(),
            It.Is<CancellationToken>(ct => ct.IsCancellationRequested)), Times.Once);
        _mockMediator.Verify(x => x.Publish(
            It.IsAny<ImageDeletedFromStorageEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockCurrentUserService.Verify(x => x.GetCompanyId(), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should respect cancellation token during commit transaction")]
    public async Task Handle_WhenCancellationTokenDuringCommitTransaction_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockCancellationToken = new CancellationToken(true);

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<ProductCategoryIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_productCategory);

        _mockImageStorageService
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()));

        _mockProductRepository
            .Setup(x => x.CreateImageAsync(It.IsAny<Image>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        _mockProductRepository
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.CommitTransactionAsync(
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)))
            .ThrowsAsync(new OperationCanceledException());

        _mockMediator
            .Setup(x => x.Publish(It.IsAny<ImageDeletedFromStorageEvent>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = async () => await _handler.Handle(_command, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.IsAny<ProductCategoryIdSpecification>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockProductRepository.Verify(x => x.CreateAsync(
            It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(
            It.Is<CancellationToken>(ct => ct.IsCancellationRequested)), Times.Once);
        _mockMediator.Verify(x => x.Publish(
            It.IsAny<ImageDeletedFromStorageEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockCurrentUserService.Verify(x => x.GetCompanyId(), Times.Once);
    }

    #endregion
}