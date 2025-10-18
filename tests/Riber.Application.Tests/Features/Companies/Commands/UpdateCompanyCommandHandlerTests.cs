using Bogus.Extensions.Brazil;
using FluentAssertions;
using Moq;
using Riber.Application.Exceptions;
using Riber.Application.Features.Companies.Commands.UpdateCompany;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Entities;
using Riber.Domain.Enums;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.Core;
using Riber.Domain.Tests;
using Riber.Domain.ValueObjects.Email;

namespace Riber.Application.Tests.Features.Companies.Commands;

public sealed class UpdateCompanyCommandHandlerTests : BaseTest
{
    #region Setup

    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICompanyRepository> _mockCompanyRepository;
    private readonly UpdateCompanyCommandHandler _commandHandler;
    private readonly Company _baseCompany;

    public UpdateCompanyCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCompanyRepository = new Mock<ICompanyRepository>();
        _commandHandler = new UpdateCompanyCommandHandler(_mockUnitOfWork.Object);

        _baseCompany = Company.Create(
            _faker.Person.FullName,
            _faker.Company.CompanyName(),
            _faker.Person.Cpf(),
            _faker.Person.Email,
            _faker.Phone.PhoneNumber("(92) 9####-####"),
            TaxIdType.IndividualWithCpf
        );
    }

    #endregion

    #region Success Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should update company successfully when all data is valid")]
    public async Task Handle_WhenAllDataIsValid_ShouldUpdateCompanySuccessfully()
    {
        // Arrange
        var request = new UpdateCompanyCommand(
            CompanyId: Guid.CreateVersion7(),
            Email: _faker.Person.Email,
            Phone: _faker.Phone.PhoneNumber("(11) 9####-####"),
            FantasyName: _faker.Company.CompanyName()
        );
        
        _mockCompanyRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_baseCompany);

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        _mockCompanyRepository
            .Setup(x => x.Update(It.IsAny<Company>()));
        
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        
        // Act
        var result = await _commandHandler.Handle(request, CancellationToken.None);

        // Assert
        result.Value.Email.Should().Be(Email.Standardization(request.Email));
        result.Value.Phone.Should().Be(request.Phone);
        result.Value.FantasyName.Should().Be(request.FantasyName);
        
        _mockCompanyRepository.Verify(x => x.GetSingleAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mockUnitOfWork.Verify(x => x.Companies, Times.Exactly(2));
        _mockCompanyRepository.Verify(x => x.Update(It.IsAny<Company>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should update only email when only email is provided")]
    public async Task Handle_WhenOnlyEmailProvided_ShouldUpdateOnlyEmail()
    {
        // Arrange
        var request = new UpdateCompanyCommand(
            CompanyId: Guid.CreateVersion7(),
            Email: _faker.Person.Email,
            Phone: string.Empty,
            FantasyName: string.Empty
        );
        
        _mockCompanyRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_baseCompany);

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        _mockCompanyRepository
            .Setup(x => x.Update(It.IsAny<Company>()));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _commandHandler.Handle(request, CancellationToken.None);

        // Assert
        result.Value.Email.Should().Be(Email.Standardization(request.Email));
        result.Value.Phone.Should().Be(_baseCompany.Phone);
        result.Value.FantasyName.Should().Be(_baseCompany.Name);
        
        _mockCompanyRepository.Verify(x => x.GetSingleAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mockUnitOfWork.Verify(x => x.Companies, Times.Once);
        _mockCompanyRepository.Verify(x => x.Update(It.IsAny<Company>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should update only phone when only phone is provided")]
    public async Task Handle_WhenOnlyPhoneProvided_ShouldUpdateOnlyPhone()
    {
        // Arrange
        var request = new UpdateCompanyCommand(
            CompanyId: Guid.CreateVersion7(),
            Email: string.Empty,
            Phone: _faker.Phone.PhoneNumber("(11) 9####-####"),
            FantasyName: string.Empty
        );
        
        _mockCompanyRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_baseCompany);

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        _mockCompanyRepository
            .Setup(x => x.Update(It.IsAny<Company>()));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _commandHandler.Handle(request, CancellationToken.None);

        // Assert
        result.Value.Email.Should().Be(_baseCompany.Email);
        result.Value.Phone.Should().Be(request.Phone);
        result.Value.FantasyName.Should().Be(_baseCompany.Name);
        
        _mockCompanyRepository.Verify(x => x.GetSingleAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mockUnitOfWork.Verify(x => x.Companies, Times.Exactly(2));
        _mockCompanyRepository.Verify(x => x.Update(It.IsAny<Company>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should update only trade name when only fantasy name is provided")]
    public async Task Handle_WhenOnlyFantasyNameProvided_ShouldUpdateOnlyFantasyName()
    {
        // Arrange
        var request = new UpdateCompanyCommand(
            CompanyId: Guid.CreateVersion7(),
            Email: string.Empty,
            Phone: string.Empty,
            FantasyName: _faker.Company.CompanyName()
        );
        
        _mockCompanyRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_baseCompany);

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        _mockCompanyRepository
            .Setup(x => x.Update(It.IsAny<Company>()));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _commandHandler.Handle(request, CancellationToken.None);

        // Assert
        result.Value.Email.Should().Be(_baseCompany.Email);
        result.Value.Phone.Should().Be(_baseCompany.Phone);
        result.Value.FantasyName.Should().Be(request.FantasyName);
        
        _mockCompanyRepository.Verify(x => x.GetSingleAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mockUnitOfWork.Verify(x => x.Companies, Times.Once);
        _mockCompanyRepository.Verify(x => x.Update(It.IsAny<Company>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Not Found Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should throw NotFoundException when company is not found")]
    public async Task Handle_WhenCompanyNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var request = new UpdateCompanyCommand(
            CompanyId: Guid.CreateVersion7(),
            Email: _faker.Person.Email,
            Phone: _faker.Phone.PhoneNumber("(11) 9####-####"),
            FantasyName: _faker.Company.CompanyName()
        );

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockCompanyRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Company?)null);

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        // Act
        var result = async () => await _commandHandler.Handle(request, CancellationToken.None);

        await result.Should()
            .ThrowExactlyAsync<NotFoundException>()
            .WithMessage(NotFoundErrors.Company);

        _mockCompanyRepository.Verify(x => x.GetSingleAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mockUnitOfWork.Verify(x => x.Companies, Times.Once);
        _mockCompanyRepository.Verify(x => x.Update(It.IsAny<Company>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Cancellation Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should respect cancellation token during company search")]
    public async Task Handle_WhenCancellationTokenDuringCompanySearch_ShouldRespectCancellationToken()
    {
        // Arrange
        var command = new UpdateCompanyCommand(
            Guid.NewGuid(),
            _faker.Person.Email,
            _faker.Phone.PhoneNumber("(11) 9####-####"),
            _faker.Company.CompanyName()
        );

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
        var result = async () => await _commandHandler.Handle(command, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockCompanyRepository.Verify(x => x.GetSingleAsync(
                It.IsAny<Specification<Company>>(),
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)),
            Times.Once);
        
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);

        _mockCompanyRepository.Verify(x => x.Update(It.IsAny<Company>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should respect cancellation token during conflict check")]
    public async Task Handle_WhenCancellationTokenDuringConflictCheck_ShouldRespectCancellationToken()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var command = new UpdateCompanyCommand(
            companyId,
            _faker.Person.Email,
            _faker.Phone.PhoneNumber("(11) 9####-####"),
            _faker.Company.CompanyName()
        );

        var mockCancellationToken = new CancellationToken(true);

        _mockCompanyRepository
            .Setup(x => x.GetSingleAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_baseCompany);

        _mockCompanyRepository
            .Setup(x => x.ExistsAsync(
                It.IsAny<Specification<Company>>(),
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)))
            .ThrowsAsync(new OperationCanceledException());

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);
        
        // Act
        var result = async () => await _commandHandler.Handle(command, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockCompanyRepository.Verify(x => x.GetSingleAsync(
            It.IsAny<Specification<Company>>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _mockCompanyRepository.Verify(x => x.ExistsAsync(
                It.IsAny<Specification<Company>>(),
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)),
            Times.Once);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);

        _mockCompanyRepository.Verify(x => x.Update(It.IsAny<Company>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion
}