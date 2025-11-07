using Bogus.Extensions.Brazil;
using FluentAssertions;
using Moq;
using Riber.Application.Abstractions.Services;
using Riber.Application.Common;
using Riber.Application.Dtos.User;
using Riber.Application.Extensions;
using Riber.Application.Features.Companies.Commands.CreateCompanyWithAdmin;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Entities;
using Riber.Domain.Entities.Company;
using Riber.Domain.Enums;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.Core;
using Riber.Domain.Tests;

namespace Riber.Application.Tests.Features.Companies.Commands;

public sealed class CreateCompanyWithAdminCommandHandlerTests : BaseTest
{
    #region Setup

    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICompanyRepository> _mockCompanyRepository;
    private readonly Mock<IUserCreationService> _mockUserCreationService;
    private readonly CreateCompanyWithAdminCommandHandler _commandHandler;
    private readonly CreateCompanyWithAdminCommand _command;

    public CreateCompanyWithAdminCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCompanyRepository = new Mock<ICompanyRepository>();
        _mockUserCreationService = new Mock<IUserCreationService>();
        _commandHandler = new CreateCompanyWithAdminCommandHandler(
            _mockUnitOfWork.Object,
            _mockUserCreationService.Object
        );

        _command = new CreateCompanyWithAdminCommand(
            CorporateName: _faker.Company.CompanyName(),
            FantasyName: _faker.Company.CompanyName(),
            TaxId: _faker.Company.Cnpj(),
            Email: _faker.Person.Email,
            Phone: _faker.Phone.PhoneNumber("(92) 9####-####"),
            Type: TaxIdType.LegalEntityWithCnpj,
            AdminFullName: _faker.Person.FullName,
            AdminUserName: _faker.Internet.UserName(),
            AdminEmail: _faker.Person.Email,
            AdminPassword: _faker.Internet.Password(),
            AdminPhoneNumber: _faker.Phone.PhoneNumber("(92) 9####-####"),
            AdminTaxId: _faker.Person.Cpf()
        );
    }

    #endregion

    #region Success Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should create company with admin successfully when all data is valid")]
    public async Task Handle_WhenAllDataIsValid_ShouldCreateCompanyWithAdminSuccessfully()
    {
        // Arrange
        _mockCompanyRepository
            .SetupSequence(x => x.ExistsAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(false);

        _mockCompanyRepository
            .Setup(x => x.CreateAsync(It.IsAny<Company>(),
                It.IsAny<CancellationToken>()));

        _mockUserCreationService
            .Setup(x => x.CreateCompleteUserAsync(
                It.IsAny<CreateUserCompleteDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<EmptyResult>());

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.HasActiveTransaction())
            .Returns(false);

        // Act
        var result = await _commandHandler.Handle(_command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.CompanyId.Should().NotBeEmpty();
        result.Value.Email.Should().BeLowerCased(_command.Email);
        result.Value.FantasyName.Should().Be(_command.FantasyName);
        result.Value.Phone.Should().Be(_command.Phone);
        result.Value.Type.Should().Be(TaxIdType.LegalEntityWithCnpj.GetDescription());
        result.Value.AdminUserEmail.Should().Be(_command.AdminEmail);
        result.Value.AdminUserName.Should().Be(_command.AdminUserName);

        _mockCompanyRepository.Verify(x => x.ExistsAsync(
            It.IsAny<Specification<Company>>(), It.IsAny<CancellationToken>()), Times.Exactly(4));

        _mockCompanyRepository.Verify(x => x.CreateAsync(
            It.IsAny<Company>(), CancellationToken.None), Times.Once);

        _mockUserCreationService.Verify(x => x.CreateCompleteUserAsync(
            It.IsAny<CreateUserCompleteDto>(), CancellationToken.None), Times.Once);

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Conflict Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return conflict when company name already exists")]
    public async Task Handle_WhenCompanyNameAlreadyExists_ShouldReturnConflict()
    {
        // Arrange
        _mockCompanyRepository
            .SetupSequence(x => x.ExistsAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(true)
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(false);

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.HasActiveTransaction())
            .Returns(true);

        // Act
        var result = await _commandHandler.Handle(_command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
        result.Error.Message.Should().Be(ConflictErrors.CorporateName);

        _mockCompanyRepository.Verify(x => x.ExistsAsync(
            It.IsAny<Specification<Company>>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockCompanyRepository.Verify(x => x.CreateAsync(
            It.IsAny<Company>(), CancellationToken.None), Times.Never);

        _mockUserCreationService.Verify(x => x.CreateCompleteUserAsync(
            It.IsAny<CreateUserCompleteDto>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return conflict when tax id already exists")]
    public async Task Handle_WhenTaxIdAlreadyExists_ShouldReturnConflict()
    {
        // Arrange
        _mockCompanyRepository
            .SetupSequence(x => x.ExistsAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false)
            .ReturnsAsync(true)
            .ReturnsAsync(false)
            .ReturnsAsync(false);

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.HasActiveTransaction())
            .Returns(true);

        // Act
        var result = await _commandHandler.Handle(_command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
        result.Error.Message.Should().Be(ConflictErrors.TaxId);

        _mockCompanyRepository.Verify(x => x.ExistsAsync(
            It.IsAny<Specification<Company>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

        _mockCompanyRepository.Verify(x => x.CreateAsync(
            It.IsAny<Company>(), CancellationToken.None), Times.Never);

        _mockUserCreationService.Verify(x => x.CreateCompleteUserAsync(
            It.IsAny<CreateUserCompleteDto>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return conflict when email already exists")]
    public async Task Handle_WhenEmailAlreadyExists_ShouldReturnConflict()
    {
        // Arrange
        _mockCompanyRepository
            .SetupSequence(x => x.ExistsAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.HasActiveTransaction())
            .Returns(true);

        // Act
        var result = await _commandHandler.Handle(_command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
        result.Error.Message.Should().Be(ConflictErrors.Email);

        _mockCompanyRepository.Verify(x => x.ExistsAsync(
            It.IsAny<Specification<Company>>(), It.IsAny<CancellationToken>()), Times.Exactly(3));

        _mockCompanyRepository.Verify(x => x.CreateAsync(
            It.IsAny<Company>(), CancellationToken.None), Times.Never);

        _mockUserCreationService.Verify(x => x.CreateCompleteUserAsync(
            It.IsAny<CreateUserCompleteDto>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return conflict when phone already exists")]
    public async Task Handle_WhenPhoneAlreadyExists_ShouldReturnConflict()
    {
        // Arrange
        _mockCompanyRepository
            .SetupSequence(x => x.ExistsAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.HasActiveTransaction())
            .Returns(true);

        // Act
        var result = await _commandHandler.Handle(_command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
        result.Error.Message.Should().Be(ConflictErrors.Phone);

        _mockCompanyRepository.Verify(x => x.ExistsAsync(
            It.IsAny<Specification<Company>>(), It.IsAny<CancellationToken>()), Times.Exactly(4));

        _mockCompanyRepository.Verify(x => x.CreateAsync(
            It.IsAny<Company>(), CancellationToken.None), Times.Never);

        _mockUserCreationService.Verify(x => x.CreateCompleteUserAsync(
            It.IsAny<CreateUserCompleteDto>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);


        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Exception Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should rollback transaction when unexpected exception occurs")]
    public async Task Handle_WhenUnexpectedExceptionOccurs_ShouldRollbackTransaction()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");

        _mockCompanyRepository
            .SetupSequence(x => x.ExistsAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(false);

        _mockCompanyRepository
            .Setup(x => x.CreateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.HasActiveTransaction())
            .Returns(true);

        // Act
        var result = async () => await _commandHandler.Handle(_command, CancellationToken.None);

        // Assert
        await result.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Test exception");

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Cancellation Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should respect cancellation token during validation")]
    public async Task Handle_WhenCancellationTokenDuringValidation_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockCancellationToken = new CancellationToken(true);

        _mockCompanyRepository
            .Setup(x => x.ExistsAsync(
                It.IsAny<Specification<Company>>(),
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)))
            .ThrowsAsync(new OperationCanceledException());

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.HasActiveTransaction())
            .Returns(true);

        // Act
        var result = async () => await _commandHandler.Handle(_command, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockCompanyRepository.Verify(x => x.ExistsAsync(
                It.IsAny<Specification<Company>>(),
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)),
            Times.Once);

        _mockCompanyRepository.Verify(x => x.CreateAsync(
            It.IsAny<Company>(), CancellationToken.None), Times.Never);

        _mockUserCreationService.Verify(x => x.CreateCompleteUserAsync(
            It.IsAny<CreateUserCompleteDto>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should respect cancellation token during commit transaction")]
    public async Task Handle_WhenCancellationTokenDuringCommitTransaction_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockCancellationToken = new CancellationToken(true);

        _mockCompanyRepository
            .SetupSequence(x => x.ExistsAsync(
                It.IsAny<Specification<Company>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(false);

        _mockCompanyRepository
            .Setup(x => x.CreateAsync(It.IsAny<Company>(), CancellationToken.None));

        _mockUserCreationService
            .Setup(x => x.CreateCompleteUserAsync(
                It.IsAny<CreateUserCompleteDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<EmptyResult>());

        _mockUnitOfWork
            .Setup(x => x.Companies)
            .Returns(_mockCompanyRepository.Object);

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.CommitTransactionAsync(It.Is<CancellationToken>(ct => ct.IsCancellationRequested)))
            .ThrowsAsync(new OperationCanceledException());

        _mockUnitOfWork
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.HasActiveTransaction())
            .Returns(true);

        // Act
        var result = async () => await _commandHandler.Handle(_command, mockCancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<OperationCanceledException>();

        _mockCompanyRepository.Verify(x => x.ExistsAsync(
            It.IsAny<Specification<Company>>(), It.IsAny<CancellationToken>()), Times.Exactly(4));

        _mockCompanyRepository.Verify(x => x.CreateAsync(
            It.IsAny<Company>(), CancellationToken.None), Times.Never);

        _mockUserCreationService.Verify(x => x.CreateCompleteUserAsync(
            It.IsAny<CreateUserCompleteDto>(), CancellationToken.None), Times.Never);

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(
            It.Is<CancellationToken>(ct => ct.IsCancellationRequested)), Times.Once);

        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}