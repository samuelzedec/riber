using Bogus.Extensions.Brazil;
using FluentAssertions;
using Moq;
using Riber.Infrastructure.Services;
using Riber.Application.Abstractions.Services;
using Riber.Application.Exceptions;
using Riber.Application.Models;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Entities;
using Riber.Domain.Repositories;
using Riber.Domain.Enums;
using Riber.Domain.Specifications.Core;
using Riber.Domain.Tests;

namespace Riber.Infrastructure.Tests.Services;

public sealed class UserCreationServiceTests : BaseTest
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserCreationService _service;
    private readonly CreateUserCompleteModel _model;
    private readonly UserDetailsModel _response;

    public UserCreationServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockAuthService = new Mock<IAuthService>();
        _mockUserRepository = new Mock<IUserRepository>();
        _service = new UserCreationService(_mockUnitOfWork.Object, _mockAuthService.Object);

        _model = CreateFaker<CreateUserCompleteModel>()
            .CustomInstantiator(f => new CreateUserCompleteModel(
                FullName: f.Person.FullName,
                UserName: f.Person.UserName,
                Email: f.Person.Email,
                Password: f.Random.String2(10),
                PhoneNumber: f.Phone.PhoneNumber("(92) 9####-####"),
                TaxId: f.Person.Cpf(),
                Position: BusinessPosition.Owner,
                Roles: ["Admin"],
                CompanyId: Guid.CreateVersion7()
            ))
            .Generate();

        _response = CreateFaker<UserDetailsModel>()
            .CustomInstantiator(f => new UserDetailsModel(
                Id: Guid.CreateVersion7(),
                UserName: f.Person.UserName,
                Email: f.Person.Email,
                EmailConfirmed: false,
                PhoneNumber: f.Phone.PhoneNumber("(92) 9####-####"),
                SecurityStamp: f.Random.AlphaNumeric(32),
                UserDomainId: Guid.Empty,
                UserDomain: null!,
                Roles: f.Make(2, () => f.Name.JobTitle()).ToList(),
                Claims: [.. f.Make(2, () => new ClaimModel(
                    Type: f.Random.Word(),
                    Value: f.Random.Word()
                ))]
            ))
            .Generate();
    }

    [Fact(DisplayName = "Should validate existing user when creating a complete user")]
    public async Task CreateCompleteUserAsync_ShouldValidateExistingUser_WhenUserAlreadyExists()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsModel?)null);

        _mockAuthService
            .Setup(x => x.FindByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsModel?)null);

        _mockAuthService
            .Setup(x => x.FindByPhoneAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsModel?)null);

        _mockUnitOfWork
            .Setup(x => x.Users)
            .Returns(_mockUserRepository.Object);

        _mockUserRepository
            .Setup(x => x.ExistsAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockUserRepository
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()));

        _mockAuthService
            .Setup(x => x.CreateAsync(It.IsAny<CreateApplicationUserModel>(), It.IsAny<CancellationToken>()));

        // Act
        await _service.CreateCompleteUserAsync(_model, CancellationToken.None);

        // Assert
        _mockAuthService.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockAuthService.Verify(x => x.FindByUserNameAsync(It.IsAny<string>()), Times.Once);
        _mockAuthService.Verify(x => x.FindByPhoneAsync(It.IsAny<string>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.Users, Times.Exactly(2));
        _mockUserRepository.Verify(
            x => x.ExistsAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockAuthService.Verify(x => x.CreateAsync(It.IsAny<CreateApplicationUserModel>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Creating user when email already exists should throw ConflictException")]
    public async Task Handle_WhenEmailAlreadyExists_ShouldThrowConflictException()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_response);

        // Act
        var result = async () => await _service.CreateCompleteUserAsync(_model, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<ConflictException>()
            .WithMessage(ConflictErrors.Email);

        _mockAuthService.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockAuthService.Verify(x => x.FindByUserNameAsync(It.IsAny<string>()), Times.Never);
        _mockAuthService.Verify(x => x.FindByPhoneAsync(It.IsAny<string>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.Users, Times.Never);
        _mockUserRepository.Verify(
            x => x.ExistsAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockAuthService.Verify(x => x.CreateAsync(It.IsAny<CreateApplicationUserModel>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact(DisplayName = "Creating user when username already exists should throw ConflictException")]
    public async Task Handle_WhenUserNameAlreadyExists_ShouldThrowConflictException()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsModel?)null);

        _mockAuthService
            .Setup(x => x.FindByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync(_response);

        // Act
        var result = async () => await _service.CreateCompleteUserAsync(_model, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<ConflictException>()
            .WithMessage(ConflictErrors.UserName);

        _mockAuthService.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockAuthService.Verify(x => x.FindByUserNameAsync(It.IsAny<string>()), Times.Once);
        _mockAuthService.Verify(x => x.FindByPhoneAsync(It.IsAny<string>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.Users, Times.Never);
        _mockUserRepository.Verify(
            x => x.ExistsAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockAuthService.Verify(x => x.CreateAsync(It.IsAny<CreateApplicationUserModel>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact(DisplayName = "Creating user when phone already exists should throw ConflictException")]
    public async Task Handle_WhenPhoneAlreadyExists_ShouldThrowConflictException()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsModel?)null);

        _mockAuthService
            .Setup(x => x.FindByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsModel?)null);

        _mockAuthService
            .Setup(x => x.FindByPhoneAsync(It.IsAny<string>()))
            .ReturnsAsync(_response);

        // Act
        var result = async () => await _service.CreateCompleteUserAsync(_model, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<ConflictException>()
            .WithMessage(ConflictErrors.Phone);

        _mockAuthService.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockAuthService.Verify(x => x.FindByUserNameAsync(It.IsAny<string>()), Times.Once);
        _mockAuthService.Verify(x => x.FindByPhoneAsync(It.IsAny<string>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.Users, Times.Never);
        _mockUserRepository.Verify(
            x => x.ExistsAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockAuthService.Verify(x => x.CreateAsync(It.IsAny<CreateApplicationUserModel>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact(DisplayName = "Creating user when tax ID already exists should throw ConflictException")]
    public async Task Handle_WhenTaxIdAlreadyExists_ShouldThrowConflictException()
    {
        // Arrange
        _mockAuthService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsModel?)null);

        _mockAuthService
            .Setup(x => x.FindByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsModel?)null);

        _mockAuthService
            .Setup(x => x.FindByPhoneAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsModel?)null);

        _mockUnitOfWork
            .Setup(x => x.Users)
            .Returns(_mockUserRepository.Object);

        _mockUserRepository
            .Setup(x => x.ExistsAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = async () => await _service.CreateCompleteUserAsync(_model, CancellationToken.None);

        // Assert
        await result.Should().ThrowExactlyAsync<ConflictException>()
            .WithMessage(ConflictErrors.TaxId);

        _mockAuthService.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockAuthService.Verify(x => x.FindByUserNameAsync(It.IsAny<string>()), Times.Once);
        _mockAuthService.Verify(x => x.FindByPhoneAsync(It.IsAny<string>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.Users, Times.Once);
        _mockUserRepository.Verify(
            x => x.ExistsAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockAuthService.Verify(x => x.CreateAsync(It.IsAny<CreateApplicationUserModel>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}