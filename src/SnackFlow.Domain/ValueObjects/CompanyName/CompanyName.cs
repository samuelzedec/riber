using SnackFlow.Domain.Constants;
using SnackFlow.Domain.ValueObjects.CompanyName.Exceptions;

namespace SnackFlow.Domain.ValueObjects.CompanyName;

public sealed record CompanyName : BaseValueObject
{
    #region Constants
    
    public const byte CorporateMaxLength = 150;
    public const byte FantasyMaxLength = 100;
    public const byte MinLength = 3;
    
    #endregion
    
    #region Properties

    public string Corporate { get; private set; }
    public string Fantasy { get; private set; }

    #endregion
    
    #region Constructors

    private CompanyName(string corporate, string fantasy)
    {
        Corporate = corporate;
        Fantasy = fantasy;
    }
    
    #endregion

    #region Factories

    public static CompanyName Create(string corporateName, string fantasyName)
    {
        CheckCorporateNameValidity(ref corporateName);
        CheckFantasyNameValidity(ref fantasyName);
        return new CompanyName(corporateName, fantasyName);
    }

    #endregion
    
    #region Private Methods

    private static void CheckCorporateNameValidity(ref string corporateName)
    {
        if (string.IsNullOrWhiteSpace(corporateName))
            throw new InvalidNameCorporateException(ErrorMessage.Name.IsNullOrEmpty);
        
        if(corporateName.Length is > CorporateMaxLength or < MinLength)
            throw new InvalidLengthCorporateNameException(ErrorMessage.Name.LengthIsInvalid(MinLength, CorporateMaxLength));
        
        corporateName = corporateName.Trim();
    }

    private static void CheckFantasyNameValidity(ref string fantasyName)
    {
        if (string.IsNullOrWhiteSpace(fantasyName))
            throw new InvalidFantasyNameException(ErrorMessage.FantasyName.IsNullOrEmpty);

        if (fantasyName.Length is > FantasyMaxLength or < MinLength)
            throw new InvalidFantasyLengthNameException(ErrorMessage.FantasyName.LengthIsInvalid(MinLength, FantasyMaxLength));
        
        fantasyName = fantasyName.Trim();
    }

    #endregion
    
    #region Operators

    public static implicit operator string(CompanyName companyName)
        => companyName.ToString();

    #endregion

    #region Overrides

    public override string ToString()
        => Fantasy;

    #endregion
}