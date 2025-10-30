using Bogus.Extensions.Brazil;
using System.Net;
using FluentAssertions;
using Moq;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Dtos.User;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Entities;
using Riber.Domain.Enums;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.Core;
using Riber.Domain.Tests;
using Riber.Infrastructure.Services.Authentication.Identity;

namespace Riber.Infrastructure.Tests.Services.Authentication.Identity;

public sealed class UserCreationServiceTests : BaseTest
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserManagementService> _mockUserManagementService;
    private readonly Mock<IUserQueryService> _mockUserQueryService;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserCreationService _service;
    private readonly CreateUserCompleteDto _dto;
    private readonly UserDetailsDto _response;

    public UserCreationServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserManagementService = new Mock<IUserManagementService>();
        _mockUserQueryService = new Mock<IUserQueryService>();
        _mockUserRepository = new Mock<IUserRepository>();
        _service = new UserCreationService(
            _mockUnitOfWork.Object, 
            _mockUserManagementService.Object, 
            _mockUserQueryService.Object
        );

        _dto = CreateFaker<CreateUserCompleteDto>()
            .CustomInstantiator(f => new CreateUserCompleteDto(
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

        _response = CreateFaker<UserDetailsDto>()
            .CustomInstantiator(f => new UserDetailsDto(
                Id: Guid.CreateVersion7(),
                UserName: f.Person.UserName,
                Email: f.Person.Email,
                EmailConfirmed: false,
                PhoneNumber: f.Phone.PhoneNumber("(92) 9####-####"),
                SecurityStamp: f.Random.AlphaNumeric(32),
                UserDomainId: Guid.Empty,
                UserDomain: null!,
                Roles: [.. f.Make(2, () => f.Name.JobTitle())],
                Claims: []
            ))
            .Generate();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should create user successfully when all validations pass")]
    public async Task CreateCompleteUserAsync_ShouldCreateUser_WhenAllValidationsPass()
    {
        // Arrange
        _mockUserQueryService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDto?)null);

        _mockUserQueryService
            .Setup(x => x.FindByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDto?)null);

        _mockUserQueryService
            .Setup(x => x.FindByPhoneAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDto?)null);

        _mockUnitOfWork
            .Setup(x => x.Users)
            .Returns(_mockUserRepository.Object);

        _mockUserRepository
            .Setup(x => x.ExistsAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockUserRepository
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUserManagementService
            .Setup(x => x.CreateUserAsync(It.IsAny<CreateApplicationUserDto>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.CreateCompleteUserAsync(_dto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        _mockUserQueryService.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockUserQueryService.Verify(x => x.FindByUserNameAsync(It.IsAny<string>()), Times.Once);
        _mockUserQueryService.Verify(x => x.FindByPhoneAsync(It.IsAny<string>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.Users, Times.Exactly(2));
        _mockUserRepository.Verify(
            x => x.ExistsAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserManagementService.Verify(x => x.CreateUserAsync(It.IsAny<CreateApplicationUserDto>()), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return conflict when email already exists")]
    public async Task Handle_WhenEmailAlreadyExists_ShouldReturnConflict()
    {
        // Arrange
        _mockUserQueryService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_response);

        // Act
        var result = await _service.CreateCompleteUserAsync(_dto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Error.Message.Should().Be(ConflictErrors.Email);

        _mockUserQueryService.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockUserQueryService.Verify(x => x.FindByUserNameAsync(It.IsAny<string>()), Times.Never);
        _mockUserQueryService.Verify(x => x.FindByPhoneAsync(It.IsAny<string>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.Users, Times.Never);
        _mockUserRepository.Verify(
            x => x.ExistsAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserManagementService.Verify(x => x.CreateUserAsync(It.IsAny<CreateApplicationUserDto>()), Times.Never);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return conflict when username already exists")]
    public async Task Handle_WhenUserNameAlreadyExists_ShouldReturnConflict()
    {
        // Arrange
        _mockUserQueryService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDto?)null);

        _mockUserQueryService
            .Setup(x => x.FindByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync(_response);

        // Act
        var result = await _service.CreateCompleteUserAsync(_dto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Error.Message.Should().Be(ConflictErrors.UserName);

        _mockUserQueryService.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockUserQueryService.Verify(x => x.FindByUserNameAsync(It.IsAny<string>()), Times.Once);
        _mockUserQueryService.Verify(x => x.FindByPhoneAsync(It.IsAny<string>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.Users, Times.Never);
        _mockUserRepository.Verify(
            x => x.ExistsAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserManagementService.Verify(x => x.CreateUserAsync(It.IsAny<CreateApplicationUserDto>()), Times.Never);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return conflict when phone already exists")]
    public async Task Handle_WhenPhoneAlreadyExists_ShouldReturnConflict()
    {
        // Arrange
        _mockUserQueryService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDto?)null);

        _mockUserQueryService
            .Setup(x => x.FindByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDto?)null);

        _mockUserQueryService
            .Setup(x => x.FindByPhoneAsync(It.IsAny<string>()))
            .ReturnsAsync(_response);

        // Act
        var result = await _service.CreateCompleteUserAsync(_dto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Error.Message.Should().Be(ConflictErrors.Phone);

        _mockUserQueryService.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockUserQueryService.Verify(x => x.FindByUserNameAsync(It.IsAny<string>()), Times.Once);
        _mockUserQueryService.Verify(x => x.FindByPhoneAsync(It.IsAny<string>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.Users, Times.Never);
        _mockUserRepository.Verify(
            x => x.ExistsAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserManagementService.Verify(x => x.CreateUserAsync(It.IsAny<CreateApplicationUserDto>()), Times.Never);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return conflict when tax ID already exists")]
    public async Task Handle_WhenTaxIdAlreadyExists_ShouldReturnConflict()
    {
        // Arrange
        _mockUserQueryService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDto?)null);

        _mockUserQueryService
            .Setup(x => x.FindByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDto?)null);

        _mockUserQueryService
            .Setup(x => x.FindByPhoneAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDto?)null);

        _mockUnitOfWork
            .Setup(x => x.Users)
            .Returns(_mockUserRepository.Object);

        _mockUserRepository
            .Setup(x => x.ExistsAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.CreateCompleteUserAsync(_dto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Error.Message.Should().Be(ConflictErrors.TaxId);

        _mockUserQueryService.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockUserQueryService.Verify(x => x.FindByUserNameAsync(It.IsAny<string>()), Times.Once);
        _mockUserQueryService.Verify(x => x.FindByPhoneAsync(It.IsAny<string>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.Users, Times.Once);
        _mockUserRepository.Verify(
            x => x.ExistsAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserManagementService.Verify(x => x.CreateUserAsync(It.IsAny<CreateApplicationUserDto>()), Times.Never);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return failure when user management service fails")]
    public async Task CreateCompleteUserAsync_ShouldReturnFailure_WhenUserManagementServiceFails()
    {
        // Arrange
        _mockUserQueryService
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDto?)null);

        _mockUserQueryService
            .Setup(x => x.FindByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDto?)null);

        _mockUserQueryService
            .Setup(x => x.FindByPhoneAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDetailsDto?)null);

        _mockUnitOfWork
            .Setup(x => x.Users)
            .Returns(_mockUserRepository.Object);

        _mockUserRepository
            .Setup(x => x.ExistsAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockUserRepository
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUserManagementService
            .Setup(x => x.CreateUserAsync(It.IsAny<CreateApplicationUserDto>()))
            .ReturnsAsync(false);

        // Act
        var result = await _service.CreateCompleteUserAsync(_dto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Message.Should().Be(UnexpectedErrors.Response);
        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        _mockUserQueryService.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockUserQueryService.Verify(x => x.FindByUserNameAsync(It.IsAny<string>()), Times.Once);
        _mockUserQueryService.Verify(x => x.FindByPhoneAsync(It.IsAny<string>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.Users, Times.Exactly(2));
        _mockUserRepository.Verify(
            x => x.ExistsAsync(It.IsAny<Specification<User>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserManagementService.Verify(x => x.CreateUserAsync(It.IsAny<CreateApplicationUserDto>()), Times.Once);
    }
}

