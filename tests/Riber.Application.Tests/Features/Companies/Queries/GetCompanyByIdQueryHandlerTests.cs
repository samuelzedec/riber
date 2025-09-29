using Bogus.Extensions.Brazil;
using FluentAssertions;
using Moq;
using Riber.Application.Exceptions;
using Riber.Application.Extensions;
using Riber.Application.Features.Companies.Queries.GetCompanyById;
using Riber.Domain.Constants;
using Riber.Domain.Entities;
using Riber.Domain.Enums;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.Core;
using Riber.Domain.Tests;
using Riber.Domain.ValueObjects.CompanyName;
using Riber.Domain.ValueObjects.Email;
using Riber.Domain.ValueObjects.Phone;
using Riber.Domain.ValueObjects.TaxId;

namespace Riber.Application.Tests.Features.Companies.Queries;

public sealed class GetCompanyByIdQueryHandlerTests : BaseTest
{
    #region Setup

    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICompanyRepository> _mockCompanyRepository;
    private readonly GetCompanyByIdQueryHandler _queryHandler;
    private readonly GetCompanyByIdQuery _query;
    private readonly Company _company;

    public GetCompanyByIdQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCompanyRepository = new Mock<ICompanyRepository>();
        _queryHandler = new GetCompanyByIdQueryHandler(_mockUnitOfWork.Object);
    
        _company = Company.Create(
            CompanyName.Create(_faker.Person.FullName, _faker.Company.CompanyName()),
            TaxId.Create(_faker.Company.Cnpj(), TaxIdType.LegalEntityWithCnpj),
            Email.Create(_faker.Person.Email.ToLowerInvariant()),
            Phone.Create(_faker.Phone.PhoneNumber("(92) 9####-####"))
        );

        _query = new GetCompanyByIdQuery(_company.Id);
    }

    #endregion

    #region Success Tests

    [Fact(DisplayName = "Should return company successfully when company exists")]
    public async Task Handle_WhenCompanyExists_ShouldReturnCompanySuccessfully()
    {
        // Arrange
        _mockCompanyRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(_company);

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        // Act
        var result = await _queryHandler.Handle(_query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.CorporateName.Should().Be(_company.Name.Corporate);
        result.Value.FantasyName.Should().Be(_company.Name.Fantasy);
        result.Value.Email.Should().Be(_company.Email);
        result.Value.Phone.Should().Be(_company.Phone);
        result.Value.TaxId.Should().Be(_company.TaxId);
        result.Value.Type.Should().Be(_company.TaxId.Type.GetDescription());

        _mockCompanyRepository.Verify(x => x.GetSingleAsync(
            It.IsAny<Specification<Company>>(), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Not Found Tests

    [Fact(DisplayName = "Should throw not found exception when company does not exist")]
    public async Task Handle_WhenCompanyDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockCompanyRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync((Company?)null);

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        // Act
        var result = async () => await _queryHandler.Handle(_query, CancellationToken.None);

        // Assert
        await result.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage(ErrorMessage.NotFound.Company);

        _mockCompanyRepository.Verify(x => x.GetSingleAsync(
            It.IsAny<Specification<Company>>(), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Should throw not found exception when company id is empty")]
    public async Task Handle_WhenCompanyIdIsEmpty_ShouldThrowNotFoundException()
    {
        // Arrange
        var emptyQuery = new GetCompanyByIdQuery(Guid.Empty);
        
        _mockCompanyRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync((Company?)null);

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        // Act
        var result = async () => await _queryHandler.Handle(emptyQuery, CancellationToken.None);

        // Assert
        await result.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage(ErrorMessage.NotFound.Company);

        _mockCompanyRepository.Verify(x => x.GetSingleAsync(
            It.IsAny<Specification<Company>>(), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Cancellation Tests

    [Fact(DisplayName = "Should respect cancellation token during repository call")]
    public async Task Handle_WhenCancellationTokenDuringRepositoryCall_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockCancellationToken = new CancellationToken(true);

        _mockCompanyRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<Specification<Company>>(),
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)))
            .ThrowsAsync(new OperationCanceledException());

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        // Act
        var result = async () => await _queryHandler.Handle(_query, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockCompanyRepository.Verify(x => x.GetSingleAsync(
                It.IsAny<Specification<Company>>(),
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)),
            Times.Once);
    }

    #endregion
    
    #region Response Mapping Tests

    [Fact(DisplayName = "Should map all company properties correctly to response")]
    public async Task Handle_WhenCompanyExists_ShouldMapAllPropertiesCorrectly()
    {
        // Arrange
        _mockCompanyRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(_company);

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        // Act
        var result = await _queryHandler.Handle(_query, CancellationToken.None);

        // Assert
        result.Value.Should().NotBeNull();
        
        result.Value.CorporateName.Should().Be(_company.Name.Corporate);
        result.Value.FantasyName.Should().Be(_company.Name.Fantasy);
        result.Value.Email.Should().Be(_company.Email);
        result.Value.Phone.Should().Be(_company.Phone);
        result.Value.TaxId.Should().Be(_company.TaxId);
        result.Value.Type.Should().Be(_company.TaxId.Type.GetDescription());
        
        result.Value.CorporateName.Should().NotBeNullOrEmpty();
        result.Value.FantasyName.Should().NotBeNullOrEmpty();
        result.Value.Email.Should().NotBeNullOrEmpty();
        result.Value.Phone.Should().NotBeNullOrEmpty();
        result.Value.TaxId.Should().NotBeNullOrEmpty();
        result.Value.Type.Should().NotBeNullOrEmpty();
    }

    #endregion
}