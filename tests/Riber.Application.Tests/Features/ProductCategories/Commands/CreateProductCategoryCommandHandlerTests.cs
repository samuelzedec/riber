using System.Linq.Expressions;
using System.Net;
using FluentAssertions;
using Moq;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Features.ProductCategories.Commands;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Constants.Messages.Entities;
using Riber.Domain.Entities.Catalog;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.Core;
using Riber.Domain.Tests;

namespace Riber.Application.Tests.Features.ProductCategories.Commands;

public sealed class CreateProductCategoryCommandHandlerTests : BaseTest
{
    #region Setup

    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly CreateProductCategoryCommandHandler _handler;
    private readonly CreateProductCategoryCommand _command;
    private readonly Guid _companyId;

    public CreateProductCategoryCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockProductRepository = new Mock<IProductRepository>();
        _companyId = Guid.NewGuid();

        _mockUnitOfWork.Setup(x => x.Products).Returns(_mockProductRepository.Object);
        _mockCurrentUserService.Setup(x => x.GetCompanyId()).Returns(_companyId);

        _handler = new CreateProductCategoryCommandHandler(
            _mockUnitOfWork.Object,
            _mockCurrentUserService.Object
        );

        _command = new CreateProductCategoryCommand(
            Code: _faker.Commerce.Categories(1).First().ToLower(),
            Name: _faker.Commerce.Department(),
            Description: _faker.Lorem.Sentence()
        );
    }

    #endregion

    #region Success Tests

    [Trait("Category", "Unit")]
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
        result.Value.Should().NotBeNull();
        result.Value!.Code.Should().Be(expectedCodeNormalized);
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

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Creating product category should normalize code to uppercase")]
    public async Task Handle_WhenCodeIsLowercase_ShouldNormalizeToUppercase()
    {
        // Arrange
        var lowercaseCommand = _command with { Code = "lowercase_code" };
        var expectedCodeNormalized = "LOWERCASE_CODE";

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
        var result = await _handler.Handle(lowercaseCommand, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Code.Should().Be(expectedCodeNormalized);

        _mockProductRepository.Verify(x => x.CreateCategoryAsync(
            It.Is<ProductCategory>(c => c.Code == expectedCodeNormalized),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Failure Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Creating product category when code already exists should return conflict failure")]
    public async Task Handle_WhenCodeAlreadyExists_ShouldReturnConflictFailure()
    {
        // Arrange
        var existingCategory = ProductCategory.Create(
            code: _command.Code.ToUpperInvariant(),
            name: "Existing Category",
            description: "Existing Description",
            companyId: _companyId
        );

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(
                It.IsAny<Specification<ProductCategory>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<ProductCategory, object>>[]>()))
            .ReturnsAsync(existingCategory);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Message.Should().Be(ConflictErrors.CategoryCode);
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);

        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.IsAny<Specification<ProductCategory>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<ProductCategory, object>>[]>()), Times.Once);

        _mockProductRepository.Verify(x => x.CreateCategoryAsync(
            It.IsAny<ProductCategory>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Creating product category when company id is invalid should return failure")]
    public async Task Handle_WhenCompanyIdIsInvalid_ShouldReturnFailure()
    {
        // Arrange
        _mockCurrentUserService
            .Setup(x => x.GetCompanyId())
            .Returns((Guid?)null);

        var handlerWithInvalidCompany = new CreateProductCategoryCommandHandler(
            _mockUnitOfWork.Object,
            _mockCurrentUserService.Object
        );

        // Act
        var result = await handlerWithInvalidCompany.Handle(_command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Message.Should().Be(CompanyErrors.Invalid);

        _mockCurrentUserService.Verify(x => x.GetCompanyId(), Times.Once);

        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.IsAny<Specification<ProductCategory>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<ProductCategory, object>>[]>()), Times.Never);

        _mockProductRepository.Verify(x => x.CreateCategoryAsync(
            It.IsAny<ProductCategory>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }


    #endregion

    #region Cancellation Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should respect cancellation token during code validation")]
    public async Task Handle_WhenCancellationTokenDuringCodeValidation_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockCancellationToken = new CancellationToken(true);

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(
                It.IsAny<Specification<ProductCategory>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<ProductCategory, object>>[]>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handler.Handle(_command, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockProductRepository.Verify(x => x.GetCategoryAsync(
            It.IsAny<Specification<ProductCategory>>(), 
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<ProductCategory, object>>[]>()), Times.Once);

        _mockProductRepository.Verify(x => x.CreateCategoryAsync(
            It.IsAny<ProductCategory>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should respect cancellation token during category creation")]
    public async Task Handle_WhenCancellationTokenDuringCategoryCreation_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockCancellationToken = new CancellationToken(true);

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(
                It.IsAny<Specification<ProductCategory>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<ProductCategory, object>>[]>()))
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
            It.IsAny<Specification<ProductCategory>>(), 
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<ProductCategory, object>>[]>()), Times.Once);

        _mockProductRepository.Verify(x => x.CreateCategoryAsync(
            It.IsAny<ProductCategory>(),
            It.Is<CancellationToken>(ct => ct.IsCancellationRequested)), Times.Once);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);

        _mockCurrentUserService.Verify(x => x.GetCompanyId(), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should respect cancellation token during save changes")]
    public async Task Handle_WhenCancellationTokenDuringSaveChanges_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockCancellationToken = new CancellationToken(true);

        _mockProductRepository
            .Setup(x => x.GetCategoryAsync(
                It.IsAny<Specification<ProductCategory>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<ProductCategory, object>>[]>()))
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
            It.IsAny<Specification<ProductCategory>>(), 
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<ProductCategory, object>>[]>()), Times.Once);

        _mockProductRepository.Verify(x => x.CreateCategoryAsync(
            It.IsAny<ProductCategory>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.Is<CancellationToken>(ct => ct.IsCancellationRequested)), Times.Once);

        _mockCurrentUserService.Verify(x => x.GetCompanyId(), Times.Once);
    }

    #endregion
}