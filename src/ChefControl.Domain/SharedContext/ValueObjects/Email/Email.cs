using System.Text.RegularExpressions;
using ChefControl.Domain.SharedContext.Constants;
using ChefControl.Domain.SharedContext.ValueObjects.Email.Exceptions;

namespace ChefControl.Domain.SharedContext.ValueObjects.Email;

public sealed partial record Email : ValueObject
{
    #region Properties

    public string Value { get; }

    #endregion

    #region Constants

    public const string EmailRegexPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

    #endregion
    
    #region Constructors

    private Email(string value)
        => Value = value;

    #endregion
    
    #region Factories

    public static Email Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value)) 
            throw new EmailNullOrEmptyException(ErrorMessage.Email.IsNullOrEmpty);

        value = value
            .Trim()
            .ToLower(); 
        
        return EmailRegex().IsMatch(value)
            ? new Email(value)
            : throw new EmailFormatInvalidException(ErrorMessage.Email.FormatInvalid);
    }    
    
    #endregion

    #region Source Generator

    [GeneratedRegex(EmailRegexPattern)]
    public static partial Regex EmailRegex();

    #endregion

    #region Operators

    public static implicit operator string(Email email) 
        => email.ToString();

    #endregion

    #region Overrides

    public override string ToString()
        => Value;

    #endregion
}