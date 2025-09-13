using System.Security.Cryptography;

namespace Riber.Domain.ValueObjects.RandomToken;

public sealed record RandomToken : BaseValueObject
{
    #region Properties

    public string Value { get; private set; }

    #endregion
    
    #region Constructors
    
    private RandomToken()
        => Value = string.Empty;
    
    private RandomToken(string value)
        => Value = value;
    
    #endregion
    
    #region Factories

    public static RandomToken Create()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return new RandomToken(Convert.ToBase64String(bytes));
    }
    
    #endregion

    #region Operators

    public static implicit operator string(RandomToken randomToken)
        => randomToken.ToString();

    #endregion

    #region Overrides

    public override string ToString()
        => Value;

    #endregion
}