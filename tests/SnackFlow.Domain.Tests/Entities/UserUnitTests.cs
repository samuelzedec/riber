using Bogus.Extensions.Brazil;
using FluentAssertions;
using SnackFlow.Domain.Entities;
using SnackFlow.Domain.Enums;

namespace SnackFlow.Domain.Tests.Entities;

public class UserUnitTests : BaseTest
{
    #region Creation Tests

    [Fact(DisplayName = "Should create user successfully with valid primitive data")]
    public void Create_WhenValidPrimitiveData_ShouldCreateSuccessfully()
    {
        // Arrange
        var fullName = _faker.Name.FullName();
        var taxId = _faker.Person.Cpf(false);
        var position = BusinessPosition.Owner;
        var applicationUserId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        // Act
        var result = User.Create(fullName, taxId, position, applicationUserId, companyId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.FullName.Value.Should().Be(fullName);
        result.TaxId.Value.Should().Be(taxId);
        result.Position.Should().Be(position);
        result.ApplicationUserId.Should().Be(applicationUserId);
        result.CompanyId.Should().Be(companyId);
        result.IsActive.Should().BeFalse();
    }

    #endregion

    #region Methods Tests

    [Fact(DisplayName = "Should activate user when company is set")]
    public void Activate_WhenCompanyIsSet_ShouldActivateUser()
    {
        // Arrange
        var user = User.Create(_faker.Name.FullName(), _faker.Person.Cpf(false), BusinessPosition.Owner, Guid.NewGuid(), Guid.NewGuid());

        // Act
        user.Activate();

        // Assert
        user.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = "Should disable user")]
    public void Disable_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var user = User.Create(_faker.Name.FullName(), _faker.Person.Cpf(false), BusinessPosition.Owner, Guid.NewGuid(), Guid.NewGuid());
        user.Activate();

        // Act
        user.Disable();

        // Assert
        user.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = "Should add company to user")]
    public void AddCompany_ShouldSetCompanyId()
    {
        // Arrange
        var user = User.Create(_faker.Name.FullName(), _faker.Person.Cpf(false), BusinessPosition.Owner, Guid.NewGuid());
        var companyId = Guid.NewGuid();

        // Act
        user.AddCompany(companyId);

        // Assert
        user.CompanyId.Should().Be(companyId);
    }

    [Fact(DisplayName = "Should remove user from company and disable user")]
    public void RemoveFromCompany_ShouldNullCompanyIdAndDisable()
    {
        // Arrange
        var user = User.Create(_faker.Name.FullName(), _faker.Person.Cpf(false), BusinessPosition.Owner, Guid.NewGuid(), Guid.NewGuid());
        user.Activate();

        // Act
        user.RemoveFromCompany();

        // Assert
        user.CompanyId.Should().BeNull();
        user.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = "Should convert user to string using operator")]
    public void Operator_String_ShouldReturnApplicationUserIdAsString()
    {
        // Arrange
        var applicationUserId = Guid.NewGuid();
        var user = User.Create(_faker.Name.FullName(), _faker.Person.Cpf(false), BusinessPosition.Owner, applicationUserId, Guid.NewGuid());

        // Act
        string userString = user;

        // Assert
        userString.Should().Be(applicationUserId.ToString());
    }

    #endregion
}