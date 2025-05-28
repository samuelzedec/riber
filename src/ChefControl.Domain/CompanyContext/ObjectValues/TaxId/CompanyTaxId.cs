using ChefControl.Domain.Companies.Enums;
using ChefControl.Domain.Shared.ObjectValues;
using ChefControl.Domain.Shared.ObjectValues.DocumentValidation;
using ChefControl.Domain.Shared.ObjectValues.DocumentValidation.Exceptions;
using ChefControl.Domain.Shared.ObjectValues.DocumentValidation.Validators;

namespace ChefControl.Domain.Companies.ObjectValues.TaxId;

public sealed record CompanyTaxId : ObjectValue
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
            _ => throw new UnsupportedCompanyTypeException("Não é possível formatar o tipo de documento.")
        };

    #endregion
}