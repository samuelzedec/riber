using Bogus.Extensions.Brazil;
using FluentAssertions;
using Riber.Domain.Entities;
using Riber.Domain.Enums;
using Riber.Domain.ValueObjects.CompanyName;
using Riber.Domain.ValueObjects.Email;
using Riber.Domain.ValueObjects.Phone;
using Riber.Domain.ValueObjects.TaxId;

namespace Riber.Domain.Tests.Entities;

public sealed class CompanyTests : BaseTest
{
    #region Creation Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should create company successfully with valid primitive data")]
    public void Create_WhenValidPrimitiveData_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = _faker.Name.FullName();
        var fantasyName = _faker.Company.CompanyName();
        var taxId = _faker.Company.Cnpj(false);
        var email = _faker.Person.Email;
        var phone = _faker.Phone.PhoneNumber("(92) 9####-####");
        var companyType = TaxIdType.LegalEntityWithCnpj;

        // Act
        var result = Company.Create(
            name,
            fantasyName,
            taxId,
            email,
            phone,
            companyType
        );

        // Assert
        result.Should().NotBeNull();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.Id.Should().NotBeEmpty();
        result.Email.Value.Should().Be(email.ToLower());
        result.Name.Corporate.Should().Be(name);
        result.Name.Fantasy.Should().Be(fantasyName);
        result.TaxId.Value.Should().Be(taxId);
        result.Phone.ToString().Should().Be(phone);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should create company successfully with valid value objects")]
    public void Create_WhenValidValueObjects_ShouldCreateSuccessfully()
    {
        // Arrange
        var companyName = CompanyName.Create(
            _faker.Name.FullName(),
            _faker.Company.CompanyName()
        );

        var companyTaxId = TaxId.Create(
            _faker.Company.Cnpj(false),
            TaxIdType.LegalEntityWithCnpj
        );

        var companyEmail = Email.Create(_faker.Person.Email);
        var companyPhone = Phone.Create(_faker.Phone.PhoneNumber("(11) 9####-####"));

        // Act
        var result = Company.Create(
            companyName,
            companyTaxId,
            companyEmail,
            companyPhone
        );

        // Assert
        result.Should().NotBeNull();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.Id.Should().NotBeEmpty();
        result.Email.Value.Should().Be(companyEmail.Value);
        result.Name.Corporate.Should().Be(companyName.Corporate);
        result.Name.Fantasy.Should().Be(companyName.Fantasy);
        result.TaxId.Value.Should().Be(companyTaxId.Value);
        result.Phone.ToString().Should().Be(companyPhone.ToString());
    }

    #endregion

    #region Update Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should update email successfully")]
    public void UpdateEmail_WhenValidEmail_ShouldUpdateSuccessfully()
    {
        // Arrange
        var company = Company.Create(
            _faker.Name.FullName(),
            _faker.Company.CompanyName(),
            _faker.Company.Cnpj(false),
            _faker.Person.Email,
            _faker.Phone.PhoneNumber("(92) 9####-####"),
            TaxIdType.LegalEntityWithCnpj
        );

        var newEmail = _faker.Person.Email;

        // Act
        company.UpdateEmail(newEmail.ToLower());

        // Assert
        company.Email.Value.Should().Be(newEmail.ToLower());
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should update phone successfully")]
    public void UpdatePhone_WhenValidPhone_ShouldUpdateSuccessfully()
    {
        // Arrange
        var company = Company.Create(
            _faker.Name.FullName(),
            _faker.Company.CompanyName(),
            _faker.Company.Cnpj(false),
            _faker.Person.Email,
            _faker.Phone.PhoneNumber("(11) 9####-####"),
            TaxIdType.LegalEntityWithCnpj
        );

        var newPhone = _faker.Phone.PhoneNumber("(11) 9####-####");

        // Act
        company.UpdatePhone(newPhone);

        // Assert
        company.Phone.ToString().Should().Be(newPhone);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should update fantasy name successfully")]
    public void UpdateFantasyName_WhenValidFantasyName_ShouldUpdateSuccessfully()
    {
        // Arrange
        var originalName = _faker.Name.FullName();
        var company = Company.Create(
            originalName,
            _faker.Company.CompanyName(),
            _faker.Company.Cnpj(false),
            _faker.Person.Email,
            _faker.Phone.PhoneNumber("(92) 9####-####"),
            TaxIdType.LegalEntityWithCnpj
        );

        var newFantasyName = _faker.Company.CompanyName();

        // Act
        company.UpdateFantasyName(newFantasyName);

        // Assert
        company.Name.Fantasy.Should().Be(newFantasyName);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should preserve original name when updating fantasy name")]
    public void UpdateFantasyName_WhenUpdatingFantasyName_ShouldPreserveOriginalName()
    {
        // Arrange
        var originalName = _faker.Name.FullName();
        var company = Company.Create(
            originalName,
            _faker.Company.CompanyName(),
            _faker.Company.Cnpj(false),
            _faker.Person.Email,
            _faker.Phone.PhoneNumber("(92) 9####-####"),
            TaxIdType.LegalEntityWithCnpj
        );

        var newFantasyName = _faker.Company.CompanyName();

        // Act
        company.UpdateFantasyName(newFantasyName);

        // Assert
        company.Name.Corporate.Should().Be(originalName);
        company.Name.Fantasy.Should().Be(newFantasyName);
    }

    #endregion
}