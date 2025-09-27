using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Riber.Application.Abstractions.Services;
using Riber.Application.Exceptions;
using Riber.Application.Features.ProductCategories.Commands;
using Riber.Domain.Constants;
using Riber.Domain.Entities;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.Core;
using Riber.Domain.Specifications.Tenants;
using Riber.Domain.Tests;

namespace Riber.Application.Tests.Features.ProductCategories.Commands;

public sealed class CreateProductCategoryCommandHandlerTests : BaseTest
{
    #region Setup

    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<ILogger<CreateProductCategoryCommandHandler>> _mockLogger;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly CreateProductCategoryCommandHandler _handler;
    private readonly CreateProductCategoryCommand _command;
    private readonly Guid _companyId;
    private readonly ProductCategory _productCategory;

    public CreateProductCategoryCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockLogger = new Mock<ILogger<CreateProductCategoryCommandHandler>>();
        _mockProductRepository = new Mock<IProductRepository>();
        _companyId = Guid.NewGuid();

        _mockUnitOfWork.Setup(x => x.Products).Returns(_mockProductRepository.Object);
        _mockCurrentUserService.Setup(x => x.GetCompanyId()).Returns(_companyId);

        _handler = new CreateProductCategoryCommandHandler(
            _mockUnitOfWork.Object,
            _mockCurrentUserService.Object,
            _mockLogger.Object
        );

        _command = new CreateProductCategoryCommand(
            Code: _faker.Commerce.Categories(1).First().ToLower(),
            Name: _faker.Commerce.Department(),
            Description: _faker.Lorem.Sentence()
        );
        
        _productCategory = ProductCategory.Create(
            code: _command.Code.ToUpperInvariant(),
            name: _command.Name,
            description: _command.Description,
            companyId: _companyId
        );
    }

    #endregion

    #region Success Tests

    [Fact(DisplayName = "Creating product category with valid data should return success response")]
    public async Task Handle_WhenValidData_ShouldReturnSuccessResponse()
    {
        // Arrange
        var expectedCodeNormalized = _command.Code.ToUpperInvariant();

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(
                It.IsAny<Specification<ProductCategory>>(), 
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<ProductCategory, object>>[]>()))
            .ReturnsAsync((ProductCategory?)null);

        _mockProductRepository
            .Setup(x => x.CreateCategoryAsync(It.IsAny<ProductCategory>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Code.Should().Be(expectedCodeNormalized);
        result.Value.Name.Should().Be(_command.Name);
        result.Value.ProductCategoryId.Should().NotBeEmpty();

        _mockProductRepository.Verify(x => x.GetCategoryAsync(
                It.IsAny<Specification<ProductCategory>>(), 
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<ProductCategory, object>>[]>()), 
            Times.Once);

        _mockProductRepository.Verify(x => x.CreateCategoryAsync(
            It.Is<ProductCategory>(c => 
                c.Code == expectedCodeNormalized &&
                c.Name == _command.Name &&
                c.Description == _command.Description &&
                c.CompanyId == _companyId),
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockCurrentUserService.Verify(x => x.GetCompanyId(), Times.Once);
    }

    [Fact(DisplayName = "Creating product category should normalize code to uppercase")]
    public async Task Handle_WhenCodeIsLowercase_ShouldNormalizeToUppercase()
    {
        // Arrange
        var lowercaseCommand = _command with { Code = "lowercase_code" };
        var expectedCodeNormalized = "LOWERCASE_CODE";

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<TenantSpecification<ProductCategory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductCategory?)null);

        _mockProductRepository
            .Setup(x => x.CreateCategoryAsync(It.IsAny<ProductCategory>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(lowercaseCommand, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be(expectedCodeNormalized);

        _mockProductRepository.Verify(x => x.CreateCategoryAsync(
            It.Is<ProductCategory>(c => c.Code == expectedCodeNormalized),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Bad Request Tests

    [Fact(DisplayName = "Creating product category when code already exists should throw BadRequestException")]
    public async Task Handle_WhenCodeAlreadyExists_ShouldThrowBadRequestException()
    {
        // Arrange
        var existingCategory = ProductCategory.Create(
            code: _command.Code.ToUpperInvariant(),
            name: "Existing Category",
            description: "Existing Description",
            companyId: _companyId
        );

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<Specification<ProductCategory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<BadRequestException>()
            .WithMessage(ErrorMessage.Product.CategoryCodeExist);

        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.IsAny<Specification<ProductCategory>>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockProductRepository.Verify(x => x.CreateCategoryAsync(
            It.IsAny<ProductCategory>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact(DisplayName = "Creating product category when code validation fails should throw BadRequestException")]
    public async Task Handle_WhenCodeValidationFails_ShouldThrowBadRequestException()
    {
        // Arrange
        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<Specification<ProductCategory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_productCategory);

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<BadRequestException>()
            .WithMessage(ErrorMessage.Product.CategoryCodeExist);

        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.IsAny<Specification<ProductCategory>>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockProductRepository.Verify(x => x.CreateCategoryAsync(
            It.IsAny<ProductCategory>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    #endregion

    #region Exception Tests

    [Fact(DisplayName = "Creating product category when current user service fails should log error and rethrow")]
    public async Task Handle_WhenCurrentUserServiceFails_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var expectedException = new BadRequestException(ErrorMessage.Invalid.CompanyId);

        _mockCurrentUserService
            .Setup(x => x.GetCompanyId())
            .Throws(expectedException);

        var handlerWithFailingUserService = new CreateProductCategoryCommandHandler(
            _mockUnitOfWork.Object,
            _mockCurrentUserService.Object,
            _mockLogger.Object
        );

        // Act
        var result = async () => await handlerWithFailingUserService.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<BadRequestException>()
            .WithMessage(ErrorMessage.Invalid.CompanyId);

        _mockCurrentUserService.Verify(x => x.GetCompanyId(), Times.Once);

        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.IsAny<Specification<ProductCategory>>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockProductRepository.Verify(x => x.CreateCategoryAsync(
            It.IsAny<ProductCategory>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact(DisplayName = "Creating product category when category creation fails should log error and rethrow")]
    public async Task Handle_WhenCategoryCreationFails_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Category creation failed");

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<Specification<ProductCategory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductCategory?)null);

        _mockProductRepository
            .Setup(x => x.CreateCategoryAsync(It.IsAny<ProductCategory>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Category creation failed");

        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.IsAny<Specification<ProductCategory>>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockProductRepository.Verify(x => x.CreateCategoryAsync(
            It.IsAny<ProductCategory>(), It.IsAny<CancellationToken>()), Times.Once);

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

    [Fact(DisplayName = "Creating product category when save changes fails should log error and rethrow")]
    public async Task Handle_WhenSaveChangesFails_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Save changes failed");

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<Specification<ProductCategory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductCategory?)null);

        _mockProductRepository
            .Setup(x => x.CreateCategoryAsync(It.IsAny<ProductCategory>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act
        var result = async () => await _handler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Save changes failed");

        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.IsAny<Specification<ProductCategory>>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockProductRepository.Verify(x => x.CreateCategoryAsync(
            It.IsAny<ProductCategory>(), It.IsAny<CancellationToken>()), Times.Once);

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

    #endregion

    #region Cancellation Tests

    [Fact(DisplayName = "Should respect cancellation token during code validation")]
    public async Task Handle_WhenCancellationTokenDuringCodeValidation_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockCancellationToken = new CancellationToken(true);

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<Specification<ProductCategory>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handler.Handle(_command, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.IsAny<Specification<ProductCategory>>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockProductRepository.Verify(x => x.CreateCategoryAsync(
            It.IsAny<ProductCategory>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Should respect cancellation token during category creation")]
    public async Task Handle_WhenCancellationTokenDuringCategoryCreation_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockCancellationToken = new CancellationToken(true);

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(It.IsAny<Specification<ProductCategory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductCategory?)null);

        _mockProductRepository
            .Setup(x => x.CreateCategoryAsync(
                It.IsAny<ProductCategory>(),
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handler.Handle(_command, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.IsAny<Specification<ProductCategory>>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockProductRepository.Verify(x => x.CreateCategoryAsync(
            It.IsAny<ProductCategory>(), 
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
            .Setup(x => x.GetCategoryAsync(It.IsAny<Specification<ProductCategory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductCategory?)null);

        _mockProductRepository
            .Setup(x => x.CreateCategoryAsync(It.IsAny<ProductCategory>(), It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handler.Handle(_command, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.IsAny<Specification<ProductCategory>>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockProductRepository.Verify(x => x.CreateCategoryAsync(
            It.IsAny<ProductCategory>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.Is<CancellationToken>(ct => ct.IsCancellationRequested)), Times.Once);

        _mockCurrentUserService.Verify(x => x.GetCompanyId(), Times.Once);
    }

    #endregion
}