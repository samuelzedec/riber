using SnackFlow.Domain.Constants;
using SnackFlow.Domain.ValueObjects.CompanyName.Exceptions;

namespace SnackFlow.Domain.ValueObjects.CompanyName;

public sealed record CompanyName : BaseValueObject
{
    #region Constants
    
    public const byte NameMaxLength = 150;
    public const byte TradingNameMaxLength = 100;
    public const byte MinLength = 3;
    
    #endregion
    
    #region Properties

    public string Name { get; private set; }
    public string TradingName { get; private set; }

    #endregion
    
    #region Constructors

    private CompanyName(string name, string tradingName)
    {
        Name = name;
        TradingName = tradingName;
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
            throw new InvalidNameException(ErrorMessage.Name.IsNullOrEmpty);
        
        if(name.Length is > NameMaxLength or < MinLength)
            throw new InvalidLengthNameException(ErrorMessage.Name.LengthIsInvalid(MinLength, NameMaxLength));
        
        name = name.Trim();
    }

    private static void CheckTradingNameValidity(ref string tradingName)
    {
        if (string.IsNullOrWhiteSpace(tradingName))
            throw new InvalidTradingNameException(ErrorMessage.TradingName.IsNullOrEmpty);

        if (tradingName.Length is > TradingNameMaxLength or < MinLength)
            throw new InvalidTradingLengthNameException(ErrorMessage.TradingName.LengthIsInvalid(MinLength, TradingNameMaxLength));
        
        tradingName = tradingName.Trim();
    }

    #endregion
    
    #region Operators

    public static implicit operator string(CompanyName companyName)
        => companyName.ToString();

    #endregion

    #region Overrides

    public override string ToString()
        => TradingName;

    #endregion
}