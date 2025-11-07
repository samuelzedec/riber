using Bogus.Extensions.Brazil;
using FluentAssertions;
using Riber.Domain.Enums;
using Riber.Domain.Specifications.Company;
using Riber.Domain.ValueObjects.Phone;

namespace Riber.Domain.Tests.Specifications.Company;

public sealed class CompanyPhoneSpecificationTests : BaseTest
{
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return true for valid phone")]
    public void Should_ReturnTrue_ForValidPhone()
    {
        // Arrange
        var phone = _faker.Phone.PhoneNumber("11987654321");
        var company = CreateCompanyWithPhone(phone);

        var specification = new CompanyPhoneSpecification(phone);

        // Act
        var result = specification.IsSatisfiedBy(company);

        // Assert
        result.Should().BeTrue();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false for different phone")]
    public void Should_ReturnFalse_ForDifferentPhone()
    {
        // Arrange
        var companyPhone = _faker.Phone.PhoneNumber("11987654321");
        var differentPhone = _faker.Phone.PhoneNumber("21987654321");

        var company = CreateCompanyWithPhone(companyPhone);
        var specification = new CompanyPhoneSpecification(differentPhone);

        // Act
        var result = specification.IsSatisfiedBy(company);

        // Assert
        result.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false for empty phone")]
    public void Should_ReturnFalse_ForEmptyPhone()
    {
        // Arrange
        var company = CreateDefaultCompany();
        var specification = new CompanyPhoneSpecification(string.Empty);

        // Act
        var result = specification.IsSatisfiedBy(company);

        // Assert
        result.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "ToExpression should be compilable")]
    public void ToExpression_Should_BeCompilable()
    {
        // Arrange
        var phone = _faker.Phone.PhoneNumber("11987654321");
        var specification = new CompanyPhoneSpecification(phone);

        // Act
        var expression = specification.ToExpression();
        var compiledExpression = expression.Compile();

        // Assert
        expression.Should().NotBeNull();
        compiledExpression.Should().NotBeNull();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Compiled expression should work same as IsSatisfiedBy")]
    public void CompiledExpression_Should_WorkSameAs_IsSatisfiedBy()
    {
        // Arrange
        var phone = _faker.Phone.PhoneNumber("11987654321");
        var company = CreateCompanyWithPhone(phone);

        var specification = new CompanyPhoneSpecification(phone);
        var compiledExpression = specification.ToExpression().Compile();

        // Act
        var resultFromMethod = specification.IsSatisfiedBy(company);
        var resultFromExpression = compiledExpression(company);

        // Assert
        resultFromMethod.Should().Be(resultFromExpression);
    }

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "Should work with different phone formats")]
    [InlineData("11987654321")]
    [InlineData("(11) 98765-4321")]
    [InlineData("11 98765-4321")]
    [InlineData("21 99999-8888")]
    public void Should_WorkWith_DifferentPhoneFormats(string phone)
    {
        // Arrange
        var company = CreateCompanyWithPhone(phone);
        var specification = new CompanyPhoneSpecification(Phone.RemoveFormatting(phone));

        // Act
        var result = specification.IsSatisfiedBy(company);

        // Assert
        result.Should().BeTrue();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should work with expression compilation")]
    public void Should_WorkWith_ExpressionCompilation()
    {
        // Arrange
        var phone1 = _faker.Phone.PhoneNumber("11987654321");
        var phone2 = _faker.Phone.PhoneNumber("21987654321");

        var company1 = CreateCompanyWithPhone(phone1);
        var company2 = CreateCompanyWithPhone(phone2);

        var specification = new CompanyPhoneSpecification(phone1);
        var predicate = specification.ToExpression().Compile();

        // Act
        var result1 = predicate(company1);
        var result2 = predicate(company2);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should handle phone with whitespace")]
    public void Should_HandlePhone_WithWhitespace()
    {
        // Arrange
        var cleanPhone = "11987654321";
        var phoneWithSpaces = " 11987654321 ";

        var company = CreateCompanyWithPhone(cleanPhone);
        var specification = new CompanyPhoneSpecification(phoneWithSpaces);

        // Act
        var result = specification.IsSatisfiedBy(company);

        // Assert
        result.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "Should work with various Brazilian phone patterns")]
    [InlineData("11999887766")]
    [InlineData("21988776655")]
    [InlineData("31977665544")]
    [InlineData("85966554433")]
    [InlineData("47955443322")]
    public void Should_WorkWith_VariousBrazilianPhonePatterns(string phone)
    {
        // Arrange
        var company = CreateCompanyWithPhone(phone);
        var specification = new CompanyPhoneSpecification(phone);

        // Act
        var result = specification.IsSatisfiedBy(company);

        // Assert
        result.Should().BeTrue();
    }

    private Domain.Entities.Company.Company CreateDefaultCompany()
        => Domain.Entities.Company.Company.Create(
            _faker.Company.CompanyName(),
            _faker.Company.CompanyName(),
            _faker.Company.Cnpj(),
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber("11987654321"),
            TaxIdType.LegalEntityWithCnpj
        );

    private Domain.Entities.Company.Company CreateCompanyWithPhone(string phone)
        => Domain.Entities.Company.Company.Create(
            _faker.Company.CompanyName(),
            _faker.Company.CompanyName(),
            _faker.Company.Cnpj(),
            _faker.Internet.Email(),
            phone,
            TaxIdType.LegalEntityWithCnpj
        );
}