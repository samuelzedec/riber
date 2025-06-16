using Bogus.Extensions.Brazil;
using ChefControl.Domain.CompanyContext.Entities;
using ChefControl.Domain.CompanyContext.Enums;
using ChefControl.Domain.CompanyContext.ValueObjects.CompanyName;
using ChefControl.Domain.CompanyContext.ValueObjects.TaxId;
using ChefControl.Domain.SharedContext.ValueObjects.Email;
using ChefControl.Domain.SharedContext.ValueObjects.Phone;
using FluentAssertions;

namespace ChefControl.Domain.Tests.CompanyContext.Entities;

public class CompanyTests : BaseTest
{
    #region Creation Tests

    [Fact(DisplayName = "Should create company successfully with valid primitive data")]
    public void ShouldCreateCompanySuccessfullyWithValidPrimitiveData()
    {
        // Arrange
        var name = _faker.Name.FullName();
        var tradingName = _faker.Company.CompanyName();
        var taxId = _faker.Company.Cnpj(false);
        var email = _faker.Person.Email;
        var phone = _faker.Phone.PhoneNumber("(##) 9####-####");
        var companyType = ECompanyType.LegalEntityWithCnpj;

        // Act
        var result = Company.Create(
            name,
            tradingName,
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
        result.CompanyName.Name.Should().Be(name);
        result.CompanyName.TradingName.Should().Be(tradingName);
        result.TaxId.Value.Should().Be(taxId);
        result.Phone.ToString().Should().Be(phone);
    }

    [Fact(DisplayName = "Should create company successfully with valid value objects")]
    public void ShouldCreateCompanySuccessfullyWithValidValueObjects()
    {
        // Arrange
        var companyName = CompanyName.Create(
            _faker.Name.FullName(),
            _faker.Company.CompanyName()
        );

        var companyTaxId = CompanyTaxId.Create(
            _faker.Company.Cnpj(false),
            ECompanyType.LegalEntityWithCnpj
        );

        var companyEmail = Email.Create(_faker.Person.Email);
        var companyPhone = Phone.Create(_faker.Phone.PhoneNumber("(##) 9####-####"));

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
        result.CompanyName.Name.Should().Be(companyName.Name);
        result.CompanyName.TradingName.Should().Be(companyName.TradingName);
        result.TaxId.Value.Should().Be(companyTaxId.Value);
        result.Phone.ToString().Should().Be(companyPhone.ToString());
    }

    #endregion

    #region Update Tests

        [Fact(DisplayName = "Should update email successfully")]
    public void ShouldUpdateEmailSuccessfully()
    {
        // Arrange
        var company = Company.Create(
            _faker.Name.FullName(),
            _faker.Company.CompanyName(),
            _faker.Company.Cnpj(false),
            _faker.Person.Email,
            _faker.Phone.PhoneNumber("(##) 9####-####"),
            ECompanyType.LegalEntityWithCnpj
        );

        var newEmail = _faker.Person.Email;

        // Act
        company.UpdateEmail(newEmail.ToLower());

        // Assert
        company.Email.Value.Should().Be(newEmail.ToLower());
    }

    [Fact(DisplayName = "Should update phone successfully")]
    public void ShouldUpdatePhoneSuccessfully()
    {
        // Arrange
        var company = Company.Create(
            _faker.Name.FullName(),
            _faker.Company.CompanyName(),
            _faker.Company.Cnpj(false),
            _faker.Person.Email,
            _faker.Phone.PhoneNumber("(11) 9####-####"),
            ECompanyType.LegalEntityWithCnpj
        );

        var newPhone = _faker.Phone.PhoneNumber("(11) 9####-####");

        // Act
        company.UpdatePhone(newPhone);

        // Assert
        company.Phone.ToString().Should().Be(newPhone);
    }

    [Fact(DisplayName = "Should update trading name successfully")]
    public void ShouldUpdateTradingNameSuccessfully()
    {
        // Arrange
        var originalName = _faker.Name.FullName();
        var company = Company.Create(
            originalName,
            _faker.Company.CompanyName(),
            _faker.Company.Cnpj(false),
            _faker.Person.Email,
            _faker.Phone.PhoneNumber("(##) 9####-####"),
            ECompanyType.LegalEntityWithCnpj
        );

        var newTradingName = _faker.Company.CompanyName();

        // Act
        company.UpdateTradingName(newTradingName);

        // Assert
        company.CompanyName.TradingName.Should().Be(newTradingName);
    }

    [Fact(DisplayName = "Should preserve original name when updating trading name")]
    public void ShouldPreserveOriginalNameWhenUpdatingTradingName()
    {
        // Arrange
        var originalName = _faker.Name.FullName();
        var company = Company.Create(
            originalName,
            _faker.Company.CompanyName(),
            _faker.Company.Cnpj(false),
            _faker.Person.Email,
            _faker.Phone.PhoneNumber("(##) 9####-####"),
            ECompanyType.LegalEntityWithCnpj
        );

        var newTradingName = _faker.Company.CompanyName();

        // Act
        company.UpdateTradingName(newTradingName);

        // Assert
        company.CompanyName.Name.Should().Be(originalName);
        company.CompanyName.TradingName.Should().Be(newTradingName);
    }

    #endregion
}