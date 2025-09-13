using Bogus.Extensions.Brazil;
using FluentAssertions;
using Riber.Domain.Constants;
using Riber.Domain.Enums;
using Riber.Domain.Exceptions;
using Riber.Domain.Validators.DocumentValidator;
using Riber.Domain.Validators.DocumentValidator.Exceptions;
using Riber.Domain.ValueObjects.TaxId;

namespace Riber.Domain.Tests.ValueObjects;

public sealed class TaxIdTests : BaseTest
{
    #region Validators

    private readonly CpfValidator _cpfValidator = new();
    private readonly CnpjValidator _cnpjValidator = new();

    #endregion

    #region Valid Creation Scenarios

    [Theory(DisplayName = "Should return TaxId instance when creating with CPF regardless of formatting")]
    [InlineData(true)]
    [InlineData(false)]
    public void Create_WhenCpfRegardlessOfFormatting_ShouldReturnTaxIdInstance(bool formatted)
    {
        // Arrange
        var cpf = _faker.Person.Cpf(formatted);
        var taxIdType = TaxIdType.IndividualWithCpf;

        // Act
        var result = TaxId.Create(cpf, taxIdType);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(_cpfValidator.Sanitize(cpf));
        result.Type.Should().Be(taxIdType);
    }

    [Theory(DisplayName = "Should return TaxId instance when creating with CNPJ regardless of formatting")]
    [InlineData(true)]
    [InlineData(false)]
    public void Create_WhenCnpjRegardlessOfFormatting_ShouldReturnTaxIdInstance(bool formatted)
    {
        // Arrange
        var cnpj = _faker.Company.Cnpj(formatted);
        var taxIdType = TaxIdType.LegalEntityWithCnpj;

        // Act
        var result = TaxId.Create(cnpj, taxIdType);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(_cnpjValidator.Sanitize(cnpj));
        result.Type.Should().Be(taxIdType);
    }

    [Theory(DisplayName = "Should return TaxId instance when creating with CPF only")]
    [InlineData(true)]
    [InlineData(false)]
    public void CreateFromCpf_WhenCpfOnly_ShouldReturnTaxIdInstance(bool formatted)
    {
        // Arrange
        var cpf = _faker.Person.Cpf(formatted);

        // Act
        var result = TaxId.CreateFromCpf(cpf);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(_cpfValidator.Sanitize(cpf));
        result.Type.Should().Be(TaxIdType.IndividualWithCpf);
    }

    [Theory(DisplayName = "Should return TaxId instance when creating with CNPJ only")]
    [InlineData(true)]
    [InlineData(false)]
    public void CreateFromCnpj_WhenCnpjOnly_ShouldReturnTaxIdInstance(bool formatted)
    {
        // Arrange
        var cpnj = _faker.Company.Cnpj(formatted);

        // Act
        var result = TaxId.CreateFromCnpj(cpnj);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(_cnpjValidator.Sanitize(cpnj));
        result.Type.Should().Be(TaxIdType.LegalEntityWithCnpj);
    }

    #endregion

    #region Invalid CPF Tests

    [Theory(DisplayName = "Should throw exception when creating with invalid CPF")]
    [InlineData("00000000000")]
    [InlineData("11111111111")]
    [InlineData("22222222222")]
    [InlineData("33333333333")]
    [InlineData("44444444444")]
    [InlineData("55555555555")]
    public void CreateFromCpf_WhenInvalidCpf_ShouldThrowException(string invalidCpf)
    {
        // Act
        var act = () => TaxId.CreateFromCpf(invalidCpf);

        // Assert
        act.Should().Throw<DomainException>();
        act.Should().ThrowExactly<InvalidCpfException>().WithMessage(ErrorMessage.Cpf.OnlyRepeatedDigits);
    }

    [Theory(DisplayName = "Should throw exception when creating with CPF having invalid verification digits")]
    [InlineData("12345678901")]
    [InlineData("98765432101")]
    [InlineData("12312312312")]
    public void CreateFromCpf_WhenCpfHasInvalidVerificationDigits_ShouldThrowException(string invalidCpf)
    {
        // Act
        var act = () => TaxId.CreateFromCpf(invalidCpf);

        // Assert
        act.Should().Throw<DomainException>();
        act.Should().ThrowExactly<InvalidCpfException>().WithMessage(ErrorMessage.Cpf.IsInvalid);
    }

    [Theory(DisplayName = "Should throw exception when creating with CPF having invalid length")]
    [InlineData("123456789")]
    [InlineData("1234567890")]
    [InlineData("123456789012")]
    [InlineData("1234567890123")]
    [InlineData("12345")]
    public void CreateFromCpf_WhenCpfHasInvalidLength_ShouldThrowException(string invalidCpf)
    {
        // Act
        var act = () => TaxId.CreateFromCpf(invalidCpf);

        // Assert
        act.Should().Throw<DomainException>();
        act.Should().ThrowExactly<InvalidLengthCpfException>().WithMessage(ErrorMessage.Cpf.LengthIsInvalid);
    }

    [Theory(DisplayName = "Should throw exception when creating with empty or whitespace CPF")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\n")]
    [InlineData("\t")]
    [InlineData("   ")]
    [InlineData("\r\n")]
    public void CreateFromCpf_WhenEmptyOrWhitespaceCpf_ShouldThrowException(string invalidCpf)
    {
        // Act
        var act = () => TaxId.CreateFromCpf(invalidCpf);

        // Assert
        act.Should().Throw<DomainException>();
        act.Should().ThrowExactly<InvalidCpfException>().WithMessage(ErrorMessage.Cpf.IsNullOrEmpty);
    }

    #endregion

    #region Invalid CNPJ Tests

    [Theory(DisplayName = "Should throw exception when creating with invalid CNPJ")]
    [InlineData("00000000000000")]
    [InlineData("11111111111111")]
    [InlineData("22222222222222")]
    [InlineData("33333333333333")]
    [InlineData("44444444444444")]
    [InlineData("55555555555555")]
    public void CreateFromCnpj_WhenInvalidCnpj_ShouldThrowException(string invalidCnpj)
    {
        // Act
        var act = () => TaxId.CreateFromCnpj(invalidCnpj);

        // Assert
        act.Should().Throw<DomainException>();
        act.Should().ThrowExactly<InvalidCnpjException>().WithMessage(ErrorMessage.Cnpj.OnlyRepeatedDigits);
    }

    [Theory(DisplayName = "Should throw exception when creating with CNPJ having invalid verification digits")]
    [InlineData("12345678000101")]
    [InlineData("98765432000101")]
    [InlineData("12312312000132")]
    public void CreateFromCnpj_WhenCnpjHasInvalidVerificationDigits_ShouldThrowException(string invalidCnpj)
    {
        // Act
        var act = () => TaxId.CreateFromCnpj(invalidCnpj);

        // Assert
        act.Should().Throw<DomainException>();
        act.Should().ThrowExactly<InvalidCnpjException>().WithMessage(ErrorMessage.Cnpj.IsInvalid);
    }

    [Theory(DisplayName = "Should throw exception when creating with CNPJ having invalid length")]
    [InlineData("12345678901")]
    [InlineData("123456789012")]
    [InlineData("123456789012345")]
    [InlineData("1234567890123456")]
    [InlineData("12345")]
    public void CreateFromCnpj_WhenCnpjHasInvalidLength_ShouldThrowException(string invalidCnpj)
    {
        // Act
        var act = () => TaxId.CreateFromCnpj(invalidCnpj);

        // Assert
        act.Should().Throw<DomainException>();
        act.Should().ThrowExactly<InvalidLengthCnpjException>().WithMessage(ErrorMessage.Cnpj.LengthIsInvalid);
    }

    [Theory(DisplayName = "Should throw exception when creating with empty or whitespace CNPJ")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\n")]
    [InlineData("\t")]
    [InlineData("   ")]
    [InlineData("\r\n")]
    public void CreateFromCnpj_WhenEmptyOrWhitespaceCnpj_ShouldThrowException(string invalidCnpj)
    {
        // Act
        var act = () => TaxId.CreateFromCnpj(invalidCnpj);

        // Assert
        act.Should().Throw<DomainException>();
        act.Should().ThrowExactly<InvalidCnpjException>().WithMessage(ErrorMessage.Cnpj.IsNullOrEmpty);
    }
    
    #endregion

    #region Overrides Tests

    [Theory(DisplayName = "Should format documents correctly with proper punctuation")]
    [InlineData("12345678909", "123.456.789-09", TaxIdType.IndividualWithCpf)]
    [InlineData("11222333000181", "11.222.333/0001-81", TaxIdType.LegalEntityWithCnpj)]
    public void ImplicitOperator_WhenFormattingDocuments_ShouldReturnProperPunctuation(string document, string expectedFormat, TaxIdType expectedType)
    {
        // Act
        var result = TaxId.Create(document, expectedType);
        string documentString = result;
        
        // Assert
        result.Value.Should().Be(document);
        result.Type.Should().Be(expectedType);
        documentString.Should().Be(expectedFormat);
    }

    #endregion
}