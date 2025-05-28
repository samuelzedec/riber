using System.Text.RegularExpressions;

using ChefControl.Domain.SharedContext.Constants;
using ChefControl.Domain.SharedContext.ObjectValues.Email.Exceptions;

namespace ChefControl.Domain.SharedContext.ObjectValues.Email;

public sealed partial record Email
{
    #region Constructors

    private Email(string value)
        => Value = value;

    #endregion

    #region Properties

    public string Value { get; }

    #endregion
    
    #region Factories

    public static Email Create(string value)
    {
        if(!string.IsNullOrWhiteSpace(value)) 
            throw new EmailNullOrEmptyException(ErrorMessage.Email.IsNullOrEmpty);

        value = value.Trim(); 
        value = value.ToLower();
        
        if (!EmailRegex().IsMatch(value))
            throw new EmailFormatInvalidException(ErrorMessage.Email.FormatInvalid);
        
        return new Email(value);
    }    
    
    #endregion

    #region Source Generator

    [GeneratedRegex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")]
    public static partial Regex EmailRegex();

    #endregion
}