using Bogus.Extensions.Brazil;
using FluentAssertions;
using Riber.Domain.Enums;
using Riber.Domain.Specifications.Company;

namespace Riber.Domain.Tests.Specifications.Company;

public sealed class CorporateNameSpecificationTests : BaseTest
{
    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return true for valid corporate name")]
    public void Should_ReturnTrue_ForValidCorporateName()
    {
        // Arrange
        var corporateName = _faker.Company.CompanyName();
        var company = CreateCompanyDefault(corporateName: corporateName);

        var specification = new CorporateNameSpecification(corporateName);

        // Act
        var result = specification.IsSatisfiedBy(company);

        // Assert
        result.Should().BeTrue();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false for different corporate name")]
    public void Should_ReturnFalse_ForDifferentCorporateName()
    {
        // Arrange
        var companyCorporateName = _faker.Company.CompanyName();
        var differentCorporateName = _faker.Company.CompanyName();

        var company = CreateCompanyDefault(corporateName: companyCorporateName);
        var specification = new CorporateNameSpecification(differentCorporateName);

        // Act
        var result = specification.IsSatisfiedBy(company);

        // Assert
        result.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should return false for empty corporate name")]
    public void Should_ReturnFalse_ForEmptyCorporateName()
    {
        // Arrange
        var company = CreateCompanyDefault();
        var specification = new CorporateNameSpecification(string.Empty);

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
        var corporateName = _faker.Company.CompanyName();
        var specification = new CorporateNameSpecification(corporateName);

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
        var corporateName = _faker.Company.CompanyName();
        var company = CreateCompanyDefault(corporateName: corporateName);

        var specification = new CorporateNameSpecification(corporateName);
        var compiledExpression = specification.ToExpression().Compile();

        // Act
        var resultFromMethod = specification.IsSatisfiedBy(company);
        var resultFromExpression = compiledExpression(company);

        // Assert
        resultFromMethod.Should().Be(resultFromExpression);
    }

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "Should work with different corporate name formats")]
    [InlineData("Empresa Exemplo LTDA")]
    [InlineData("Tecnologia & Inovação S.A.")]
    [InlineData("Comércio de Produtos Diversos EIRELI")]
    [InlineData("Indústria Brasileira de Componentes")]
    [InlineData("Serviços Especializados ME")]
    public void Should_WorkWith_DifferentCorporateNameFormats(string corporateName)
    {
        // Arrange
        var company = CreateCompanyDefault(corporateName: corporateName);
        var specification = new CorporateNameSpecification(corporateName);

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
        var corporateName1 = _faker.Company.CompanyName();
        var corporateName2 = _faker.Company.CompanyName();

        var company1 = CreateCompanyDefault(corporateName: corporateName1);
        var company2 = CreateCompanyDefault(corporateName: corporateName2);

        var specification = new CorporateNameSpecification(corporateName1);
        var predicate = specification.ToExpression().Compile();

        // Act
        var result1 = predicate(company1);
        var result2 = predicate(company2);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should be case sensitive")]
    public void Should_BeCaseSensitive()
    {
        // Arrange
        var lowerCorporateName = "empresa exemplo ltda";
        var upperCorporateName = "EMPRESA EXEMPLO LTDA";

        var company = CreateCompanyDefault(corporateName: lowerCorporateName);
        var specification = new CorporateNameSpecification(upperCorporateName);

        // Act
        var result = specification.IsSatisfiedBy(company);

        // Assert
        result.Should().BeFalse();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should handle corporate name with whitespace")]
    public void Should_HandleCorporateName_WithWhitespace()
    {
        // Arrange
        var cleanCorporateName = "Empresa Exemplo LTDA";
        var corporateNameWithSpaces = " Empresa Exemplo LTDA ";

        var company = CreateCompanyDefault(corporateName: cleanCorporateName);
        var specification = new CorporateNameSpecification(corporateNameWithSpaces);

        // Act
        var result = specification.IsSatisfiedBy(company);

        // Assert
        result.Should().BeFalse(); // Assumindo que não faz trim automático
    }

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "Should work with various business name patterns")]
    [InlineData("ABC Tecnologia LTDA")]
    [InlineData("XYZ Comércio e Serviços S.A.")]
    [InlineData("Produtos Inovadores EIRELI")]
    [InlineData("Consultoria Empresarial ME")]
    [InlineData("Desenvolvimento de Software EPP")]
    public void Should_WorkWith_VariousBusinessNamePatterns(string corporateName)
    {
        // Arrange
        var company = CreateCompanyDefault(corporateName: corporateName);
        var specification = new CorporateNameSpecification(corporateName);

        // Act
        var result = specification.IsSatisfiedBy(company);

        // Assert
        result.Should().BeTrue();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should handle special characters in corporate name")]
    public void Should_HandleSpecialCharacters_InCorporateName()
    {
        // Arrange
        var corporateNameWithSpecialChars = "Empresa & Cia. - Soluções Ltda.";
        var company = CreateCompanyDefault(corporateName: corporateNameWithSpecialChars);
        var specification = new CorporateNameSpecification(corporateNameWithSpecialChars);

        // Act
        var result = specification.IsSatisfiedBy(company);

        // Assert
        result.Should().BeTrue();
    }

    private Domain.Entities.Company.Company CreateCompanyDefault(
        string? taxId = null,
        string? email = null,
        string? phone = null,
        string? corporateName = null)
        => Domain.Entities.Company.Company.Create(
            corporateName ?? _faker.Company.CompanyName(),
            _faker.Company.CompanyName(),
            taxId ?? _faker.Company.Cnpj(),
            email ?? _faker.Internet.Email(),
            phone ?? _faker.Phone.PhoneNumber("11987654321"),
            TaxIdType.LegalEntityWithCnpj
        );
}