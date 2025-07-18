using SnackFlow.Domain.Abstractions;
using SnackFlow.Domain.Enums;
using SnackFlow.Domain.Validators.DocumentValidator;

namespace SnackFlow.Domain.ValueObjects.TaxId;

public sealed record TaxId : BaseValueObject
{
    #region Properties

    public string Value { get; private set; }
    public TaxIdType Type { get; private set; }

    #endregion

    #region Constructors

    private TaxId(string value, TaxIdType type)
    {
        Value = value;
        Type = type;
    }

    #endregion

    #region Factories
    
    public static TaxId Create(string value, TaxIdType type)
    {
        IDocumentValidator validator = type is TaxIdType.IndividualWithCpf
            ? new CpfValidator()
            : new CnpjValidator();
        
        validator.IsValid(value);
        return new TaxId(validator.Sanitize(value), type);
    }
    
    public static TaxId CreateFromCpf(string cpf)
    {
        var validator = new CpfValidator();
        validator.IsValid(cpf);
        return new TaxId(validator.Sanitize(cpf), TaxIdType.IndividualWithCpf);
    }

    public static TaxId CreateFromCnpj(string cnpj)
    {
        var validator = new CnpjValidator();
        validator.IsValid(cnpj);
        return new TaxId(validator.Sanitize(cnpj), TaxIdType.LegalEntityWithCnpj);
    }

    #endregion
    
    #region Operators

    public static implicit operator string(TaxId taxId) 
        => taxId.ToString();

    #endregion

    #region Overrides

    public override string ToString()
        => Type == TaxIdType.IndividualWithCpf
            ? CpfValidator.Format(Value)
            : CnpjValidator.Format(Value);

    #endregion
}