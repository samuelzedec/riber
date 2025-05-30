using System.Text.RegularExpressions;

using ChefControl.Domain.SharedContext.Constants;
using ChefControl.Domain.SharedContext.ObjectValues.Phone.Exceptions;

namespace ChefControl.Domain.SharedContext.ObjectValues.Phone;

public sealed partial record Phone : ObjectValue
{
    #region Properties

    public string Value { get; }

    #endregion

    #region Constants

    public const string PhoneRegexPattern = @"^(?:\(?(?:11|[12-9][0-9])\)?\s?)?9[0-9]{4}[\s\-]?[0-9]{4}$";

    #endregion

    #region Constructors

    private Phone(string value)
        => Value = value;

    #endregion

    #region Factories

    public static Phone Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new PhoneNullOrEmptyException(ErrorMessage.Phone.IsNullOrEmpty);

        value = value.Trim();

        return PhoneRegex().IsMatch(value)
            ? new Phone(RemoveFormatting(value))
            : throw new PhoneFormatInvalidException(ErrorMessage.Phone.FormatInvalid);
    }

    #endregion

    #region Source Generator

    [GeneratedRegex(PhoneRegexPattern)]
    public static partial Regex PhoneRegex();

    #endregion

    #region Operators

    public static implicit operator string(Phone phone)
        => phone.ToString();

    #endregion

    #region Overrides

    public override string ToString()
        => Value.Length switch
        {
            11 => $"({Value[..2]}) {Value[2..7]}-{Value[7..]}",
            10 => $"({Value[..2]}) {Value[2..6]}-{Value[6..]}",
            _ => throw new PhoneFormatInvalidException(ErrorMessage.Phone.FormatInvalid)
        };

    #endregion

    #region Private Methods

    public static string RemoveFormatting(string value)
        => new([..value.Where(char.IsDigit)]);

    #endregion
}