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

    public static CompanyName Create(string name, string tradingName)
    {
        CheckNameValidity(ref name);
        CheckTradingNameValidity(ref tradingName);
        return new CompanyName(name, tradingName);
    }

    #endregion
    
    #region Private Methods

    private static void CheckNameValidity(ref string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidNameCorporateException(ErrorMessage.Name.IsNullOrEmpty);
        
        if(name.Length is > CorporateMaxLength or < MinLength)
            throw new InvalidLengthCorporateNameException(ErrorMessage.Name.LengthIsInvalid(MinLength, CorporateMaxLength));
        
        name = name.Trim();
    }

    private static void CheckTradingNameValidity(ref string tradingName)
    {
        if (string.IsNullOrWhiteSpace(tradingName))
            throw new InvalidFantasyNameException(ErrorMessage.FantasyName.IsNullOrEmpty);

        if (tradingName.Length is > FantasyMaxLength or < MinLength)
            throw new InvalidTradingLengthNameException(ErrorMessage.FantasyName.LengthIsInvalid(MinLength, FantasyMaxLength));
        
        tradingName = tradingName.Trim();
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