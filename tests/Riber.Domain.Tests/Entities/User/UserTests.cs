using Bogus.Extensions.Brazil;
using FluentAssertions;
using Riber.Domain.Enums;
using Riber.Domain.Exceptions;

namespace Riber.Domain.Tests.Entities.User;

public sealed class UserTests : BaseTest
{
    #region Creation Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should create user successfully with valid primitive data")]
    public void Create_WhenValidPrimitiveData_ShouldCreateSuccessfully()
    {
        // Arrange
        var fullName = _faker.Name.FullName();
        var taxId = _faker.Person.Cpf(false);
        var position = _faker.Random.Enum<BusinessPosition>();
        var companyId = Guid.NewGuid();

        // Act
        var result = Riber.Domain.Entities.User.User.Create(
            fullName,
            taxId,
            position,
            companyId
        );

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.FullName.Value.Should().Be(fullName);
        result.TaxId.Value.Should().Be(taxId);
        result.Position.Should().Be(position);
        result.CompanyId.Should().Be(companyId);
        result.IsActive.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should create user successfully without company")]
    public void Create_WhenNoCompanyId_ShouldCreateSuccessfully()
    {
        // Arrange
        var fullName = _faker.Name.FullName();
        var taxId = _faker.Person.Cpf(false);
        var position = _faker.Random.Enum<BusinessPosition>();

        // Act
        var result = Riber.Domain.Entities.User.User.Create(
            fullName,
            taxId,
            position
        );

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CompanyId.Should().BeNull();
        result.IsActive.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "Should create user with different business positions")]
    [InlineData(BusinessPosition.Employee)]
    [InlineData(BusinessPosition.Manager)]
    [InlineData(BusinessPosition.Director)]
    [InlineData(BusinessPosition.Owner)]
    public void Create_WhenDifferentPositions_ShouldCreateSuccessfully(BusinessPosition position)
    {
        // Arrange
        var fullName = _faker.Name.FullName();
        var taxId = _faker.Person.Cpf(false);

        // Act
        var result = Riber.Domain.Entities.User.User.Create(
            fullName,
            taxId,
            position
        );

        // Assert
        result.Should().NotBeNull();
        result.Position.Should().Be(position);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should create user with inactive status by default")]
    public void Create_WhenCreated_ShouldBeInactiveByDefault()
    {
        // Arrange
        var fullName = _faker.Name.FullName();
        var taxId = _faker.Person.Cpf(false);
        var position = _faker.Random.Enum<BusinessPosition>();

        // Act
        var result = Riber.Domain.Entities.User.User.Create(
            fullName,
            taxId,
            position
        );

        // Assert
        result.IsActive.Should().BeFalse();
    }

    #endregion

    #region Activate Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should activate user when company is assigned")]
    public void Activate_WhenCompanyIsAssigned_ShouldActivateSuccessfully()
    {
        // Arrange
        var user = CreateUserWithCompany();

        // Act
        user.Activate();

        // Assert
        user.IsActive.Should().BeTrue();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should throw exception when activating user without company")]
    public void Activate_WhenNoCompany_ShouldThrowIdentifierNullException()
    {
        // Arrange
        var user = CreateUserWithoutCompany();

        // Act
        var act = () => user.Activate();

        // Assert
        act.Should().Throw<IdentifierNullException>();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should keep user active after multiple activations")]
    public void Activate_WhenCalledMultipleTimes_ShouldRemainActive()
    {
        // Arrange
        var user = CreateUserWithCompany();

        // Act
        user.Activate();
        user.Activate();

        // Assert
        user.IsActive.Should().BeTrue();
    }

    #endregion

    #region Disable Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should disable active user")]
    public void Disable_WhenUserIsActive_ShouldDisableSuccessfully()
    {
        // Arrange
        var user = CreateUserWithCompany();
        user.Activate();

        // Act
        user.Disable();

        // Assert
        user.IsActive.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should disable inactive user")]
    public void Disable_WhenUserIsInactive_ShouldRemainInactive()
    {
        // Arrange
        var user = CreateUserWithCompany();

        // Act
        user.Disable();

        // Assert
        user.IsActive.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should keep company reference when disabling")]
    public void Disable_WhenCalled_ShouldKeepCompanyReference()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var user = CreateUserWithCompany(companyId);
        user.Activate();

        // Act
        user.Disable();

        // Assert
        user.IsActive.Should().BeFalse();
        user.CompanyId.Should().Be(companyId);
    }

    #endregion

    #region AddCompany Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should add company to user")]
    public void AddCompany_WhenValidCompanyId_ShouldAddCompanySuccessfully()
    {
        // Arrange
        var user = CreateUserWithoutCompany();
        var companyId = Guid.NewGuid();

        // Act
        user.AddCompany(companyId);

        // Assert
        user.CompanyId.Should().Be(companyId);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should replace existing company")]
    public void AddCompany_WhenUserAlreadyHasCompany_ShouldReplaceCompany()
    {
        // Arrange
        var oldCompanyId = Guid.NewGuid();
        var newCompanyId = Guid.NewGuid();
        var user = CreateUserWithCompany(oldCompanyId);

        // Act
        user.AddCompany(newCompanyId);

        // Assert
        user.CompanyId.Should().Be(newCompanyId);
        user.CompanyId.Should().NotBe(oldCompanyId);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should add company and allow activation")]
    public void AddCompany_WhenAddedToUserWithoutCompany_ShouldAllowActivation()
    {
        // Arrange
        var user = CreateUserWithoutCompany();
        var companyId = Guid.NewGuid();

        // Act
        user.AddCompany(companyId);
        var act = () => user.Activate();

        // Assert
        act.Should().NotThrow();
        user.IsActive.Should().BeTrue();
    }

    #endregion

    #region RemoveFromCompany Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should remove user from company")]
    public void RemoveFromCompany_WhenUserHasCompany_ShouldRemoveCompanySuccessfully()
    {
        // Arrange
        var user = CreateUserWithCompany();
        user.Activate();

        // Act
        user.RemoveFromCompany();

        // Assert
        user.CompanyId.Should().BeNull();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should disable user when removing from company")]
    public void RemoveFromCompany_WhenUserIsActive_ShouldDisableUser()
    {
        // Arrange
        var user = CreateUserWithCompany();
        user.Activate();

        // Act
        user.RemoveFromCompany();

        // Assert
        user.IsActive.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should remove company and disable even if user was already inactive")]
    public void RemoveFromCompany_WhenUserIsInactive_ShouldRemoveCompanyAndKeepInactive()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var user = CreateUserWithCompany(companyId);

        // Act
        user.RemoveFromCompany();

        // Assert
        user.CompanyId.Should().BeNull();
        user.IsActive.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should prevent activation after removing from company")]
    public void RemoveFromCompany_WhenRemoved_ShouldPreventActivation()
    {
        // Arrange
        var user = CreateUserWithCompany();
        user.Activate();

        // Act
        user.RemoveFromCompany();
        var act = () => user.Activate();

        // Assert
        user.CompanyId.Should().BeNull();
        user.IsActive.Should().BeFalse();
        act.Should().Throw<IdentifierNullException>();
    }

    #endregion

    #region Invalid Creation Tests

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "Should throw exception when creating with invalid full name")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WhenInvalidFullName_ShouldThrowException(string? invalidName)
    {
        // Arrange
        var taxId = _faker.Person.Cpf(false);
        var position = _faker.Random.Enum<BusinessPosition>();

        // Act
        var act = () => Riber.Domain.Entities.User.User.Create(
            invalidName!,
            taxId,
            position
        );

        // Assert
        act.Should().Throw<Exception>();
    }

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "Should throw exception when creating with invalid tax ID")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123")]
    [InlineData("invalid-cpf")]
    [InlineData(null)]
    public void Create_WhenInvalidTaxId_ShouldThrowException(string? invalidTaxId)
    {
        // Arrange
        var fullName = _faker.Name.FullName();
        var position = _faker.Random.Enum<BusinessPosition>();

        // Act
        var act = () => Riber.Domain.Entities.User.User.Create(
            fullName,
            invalidTaxId!,
            position
        );

        // Assert
        act.Should().Throw<Exception>();
    }

    #endregion

    #region Helper Methods

    private Riber.Domain.Entities.User.User CreateUserWithCompany(Guid? companyId)
    {
        var fullName = _faker.Name.FullName();
        var taxId = _faker.Person.Cpf(false);
        var position = _faker.Random.Enum<BusinessPosition>();

        return Riber.Domain.Entities.User.User.Create(
            fullName,
            taxId,
            position,
            companyId ?? Guid.NewGuid()
        );
    }

    private Riber.Domain.Entities.User.User CreateUserWithCompany()
        => CreateUserWithCompany(null);

    private Riber.Domain.Entities.User.User CreateUserWithoutCompany()
    {
        var fullName = _faker.Name.FullName();
        var taxId = _faker.Person.Cpf(false);
        var position = _faker.Random.Enum<BusinessPosition>();

        return Riber.Domain.Entities.User.User.Create(
            fullName,
            taxId,
            position
        );
    }

    #endregion
}

