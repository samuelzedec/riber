using System.Linq.Expressions;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Moq;
using SnackFlow.Application.Exceptions;
using SnackFlow.Application.Extensions;
using SnackFlow.Application.Features.Companies.Commands.CreateCompany;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Entities;
using SnackFlow.Domain.Enums;
using SnackFlow.Domain.Repositories;
using SnackFlow.Domain.Tests;

namespace SnackFlow.Application.Tests.Features.Companies.Commands;

public class CreateCompanyCommandHandlerUnitTests : BaseTest
{
    #region Setup

    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICompanyRepository> _mockCompanyRepository;
    private readonly CreateCompanyCommandHandler _commandHandler;
    private readonly CreateCompanyCommand _command;

    public CreateCompanyCommandHandlerUnitTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCompanyRepository = new Mock<ICompanyRepository>();
        _commandHandler = new CreateCompanyCommandHandler(_mockUnitOfWork.Object);
        
        _command = new CreateCompanyCommand(
            _faker.Person.FullName,
            _faker.Company.CompanyName(),
            _faker.Company.Cnpj(),
            _faker.Person.Email,
            _faker.Phone.PhoneNumber("(92) 9####-####"),
            TaxIdType.LegalEntityWithCnpj
        );
    }

    #endregion

    #region Success Tests

    [Fact(DisplayName = "Should create company successfully when all data is valid")]
    public async Task Handle_WhenAllDataIsValid_ShouldCreateCompanySuccessfully()
    {
        // Arrange
        _mockCompanyRepository
            .SetupSequence(x => x.ExistsAsync(
                It.IsAny<Expression<Func<Company, bool>>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(false);

        _mockCompanyRepository
            .Setup(x => x.CreateAsync(It.IsAny<Company>()));

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _commandHandler.Handle(_command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.CompanyId.Should().NotBeEmpty();
        result.Value.Email.Should().BeLowerCased(_command.Email);
        result.Value.FantasyName.Should().Be(_command.FantasyName);
        result.Value.Phone.Should().Be(_command.Phone);
        result.Value.Type.Should().Be(TaxIdType.LegalEntityWithCnpj.GetDescription());

        _mockCompanyRepository.Verify(x => x.ExistsAsync(
            It.IsAny<Expression<Func<Company, bool>>>(), It.IsAny<CancellationToken>()), Times.Exactly(4));

        _mockCompanyRepository.Verify(x => x.CreateAsync(
            It.IsAny<Company>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Conflict Tests

    [Fact(DisplayName = "Should throw conflict exception when company name already exists")]
    public async Task Handle_WhenCompanyNameAlreadyExists_ShouldThrowConflictException()
    {
        // Arrange
        _mockCompanyRepository
            .SetupSequence(x => x.ExistsAsync(
                It.IsAny<Expression<Func<Company, bool>>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(true)
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(false);

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        // Act
        var result = async () => await _commandHandler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage(ErrorMessage.Conflict.CorporateNameAlreadyExists);

        _mockCompanyRepository.Verify(x => x.ExistsAsync(
            It.IsAny<Expression<Func<Company, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockCompanyRepository.Verify(x => x.CreateAsync(
            It.IsAny<Company>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Should throw conflict exception when tax id already exists")]
    public async Task Handle_WhenTaxIdAlreadyExists_ShouldThrowConflictException()
    {
        // Arrange
        _mockCompanyRepository
            .SetupSequence(x => x.ExistsAsync(
                It.IsAny<Expression<Func<Company, bool>>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false)
            .ReturnsAsync(true)
            .ReturnsAsync(false)
            .ReturnsAsync(false);

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        // Act
        var result = async () => await _commandHandler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage(ErrorMessage.Conflict.TaxIdAlreadyExists);

        _mockCompanyRepository.Verify(x => x.ExistsAsync(
            It.IsAny<Expression<Func<Company, bool>>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

        _mockCompanyRepository.Verify(x => x.CreateAsync(
            It.IsAny<Company>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Should throw conflict exception when email already exists")]
    public async Task Handle_WhenEmailAlreadyExists_ShouldThrowConflictException()
    {
        // Arrange
        _mockCompanyRepository
            .SetupSequence(x => x.ExistsAsync(
                It.IsAny<Expression<Func<Company, bool>>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        // Act
        var result = async () => await _commandHandler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage(ErrorMessage.Conflict.EmailAlreadyExists);

        _mockCompanyRepository.Verify(x => x.ExistsAsync(
            It.IsAny<Expression<Func<Company, bool>>>(), It.IsAny<CancellationToken>()), Times.Exactly(3));

        _mockCompanyRepository.Verify(x => x.CreateAsync(
            It.IsAny<Company>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Should throw conflict exception when phone already exists")]
    public async Task Handle_WhenPhoneAlreadyExists_ShouldThrowConflictException()
    {
        // Arrange
        _mockCompanyRepository
            .SetupSequence(x => x.ExistsAsync(
                It.IsAny<Expression<Func<Company, bool>>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        // Act
        var result = async () => await _commandHandler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage(ErrorMessage.Conflict.PhoneAlreadyExists);

        _mockCompanyRepository.Verify(x => x.ExistsAsync(
            It.IsAny<Expression<Func<Company, bool>>>(), It.IsAny<CancellationToken>()), Times.Exactly(4));

        _mockCompanyRepository.Verify(x => x.CreateAsync(
            It.IsAny<Company>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Cancellation Tests

    [Fact(DisplayName = "Should respect cancellation token during validation")]
    public async Task Handle_WhenCancellationTokenDuringValidation_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockCancellationToken = new CancellationToken(true);

        _mockCompanyRepository
            .Setup(x => x.ExistsAsync(
                It.IsAny<Expression<Func<Company, bool>>>(),
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)))
            .ThrowsAsync(new OperationCanceledException());

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        // Act
        var result = async () => await _commandHandler.Handle(_command, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockCompanyRepository.Verify(x => x.ExistsAsync(
                It.IsAny<Expression<Func<Company, bool>>>(),
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)),
            Times.Once);

        _mockCompanyRepository.Verify(x => x.CreateAsync(
            It.IsAny<Company>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Should respect cancellation token during save changes")]
    public async Task Handle_WhenCancellationTokenDuringSaveChanges_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockCancellationToken = new CancellationToken(true);

        _mockCompanyRepository
            .SetupSequence(x => x.ExistsAsync(
                It.IsAny<Expression<Func<Company, bool>>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(false);

        _mockCompanyRepository
            .Setup(x => x.CreateAsync(It.IsAny<Company>()));

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.Is<CancellationToken>(ct => ct.IsCancellationRequested)))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _commandHandler.Handle(_command, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockCompanyRepository.Verify(x => x.ExistsAsync(
            It.IsAny<Expression<Func<Company, bool>>>(), It.IsAny<CancellationToken>()), Times.Exactly(4));

        _mockCompanyRepository.Verify(x => x.CreateAsync(
            It.IsAny<Company>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(
            It.Is<CancellationToken>(ct => ct.IsCancellationRequested)), Times.Once);
    }

    #endregion
}