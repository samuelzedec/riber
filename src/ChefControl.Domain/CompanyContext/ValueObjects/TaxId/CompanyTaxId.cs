using ChefControl.Domain.CompanyContext.Enums;
using ChefControl.Domain.SharedContext.Constants;
using ChefControl.Domain.SharedContext.ValueObjects;
using ChefControl.Domain.SharedContext.ValueObjects.DocumentValidation;
using ChefControl.Domain.SharedContext.ValueObjects.DocumentValidation.Exceptions;
using ChefControl.Domain.SharedContext.ValueObjects.DocumentValidation.Validators;

namespace ChefControl.Domain.CompanyContext.ValueObjects.TaxId;

public sealed record CompanyTaxId : ValueObject
{
    #region Properties

    public string Value { get; private set; }
    public ECompanyType Type { get; private set; }

    #endregion

    #region Constructors

    private CompanyTaxId(string value, ECompanyType type)
    {
        Value = value;
        Type = type;
    }

    #endregion

    #region Factories
    
    public static CompanyTaxId Create(string value, ECompanyType type)
    {
        IDocumentValidator validator = type is ECompanyType.IndividualWithCpf
            ? new CpfValidator()
            : new CnpjValidator();
        
        validator.IsValid(value);
        return new CompanyTaxId(value, type);
    }
    
    public static CompanyTaxId CreateFromCpf(string cpf)
    {
        var validator = new CpfValidator();
        validator.IsValid(cpf);
        return new CompanyTaxId(validator.Sanitize(cpf), ECompanyType.IndividualWithCpf);
    }

    public static CompanyTaxId CreateFromCnpj(string cnpj)
    {
        var validator = new CnpjValidator();
        validator.IsValid(cnpj);
        return new CompanyTaxId(validator.Sanitize(cnpj), ECompanyType.LegalEntityWithCnpj);
    }

    #endregion
    
    #region Operators

    public static implicit operator string(CompanyTaxId companyTaxId) 
        => companyTaxId.ToString();

    #endregion

    #region Overrides

    public override string ToString()
        => Type switch
        {
            ECompanyType.IndividualWithCpf => CpfValidator.Format(Value),
            ECompanyType.LegalEntityWithCnpj => CnpjValidator.Format(Value),
            _ => throw new UnsupportedCompanyTypeException(ErrorMessage.Document.IsInvalid)
        };

    #endregion
}