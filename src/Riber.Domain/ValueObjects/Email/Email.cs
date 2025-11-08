using System.Text.RegularExpressions;
using Riber.Domain.Constants.Messages.ValueObjects;
using Riber.Domain.ValueObjects.Email.Exceptions;

namespace Riber.Domain.ValueObjects.Email;

public sealed partial record Email : BaseValueObject
{
    #region Properties

    public string Value { get; }

    #endregion

    #region Constants

    public const string RegexPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

    #endregion

    #region Constructors

    private Email(string value)
        => Value = value;

    #endregion

    #region Factories

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new EmptyEmailException(EmailErrors.Empty);

        value = Standardization(value);

        return EmailRegex().IsMatch(value)
            ? new Email(value)
            : throw new InvalidEmailFormatException(EmailErrors.Format);
    }

    #endregion

    #region Source Generator

    [GeneratedRegex(RegexPattern)]
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

    #region Public Methods

    public static string Standardization(string value)
        => value.Trim().ToLower();

    #endregion
}