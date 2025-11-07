using System.Text.RegularExpressions;
using Riber.Domain.Constants.Messages.ValueObjects;
using Riber.Domain.ValueObjects.FullName.Exceptions;

namespace Riber.Domain.ValueObjects.FullName;

public sealed partial record FullName : BaseValueObject
{
    #region Constants

    public const byte MaxLength = 255;
    public const byte MinLength = 5;
    public const string RegexPattern = @"^[A-Za-zÀ-ÖØ-öø-ÿ' ]+$";

    #endregion

    #region Properties

    public string Value { get; private set; }

    #endregion

    #region Constructors

    private FullName(string value)
        => Value = value;

    #endregion

    #region Factories

    public static FullName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidFullNameException(NameErrors.FullNameEmpty);

        value = value.Trim();

        if (value.Length is > MaxLength or < MinLength)
            throw new InvalidFullNameLengthException(NameErrors.FullNameLength(MinLength, MaxLength));

        return FullNameRegex().IsMatch(value)
            ? new FullName(value)
            : throw new InvalidFullNameException(NameErrors.FullNameFormat);
    }

    #endregion

    #region Source Generator

    [GeneratedRegex(RegexPattern)]
    public static partial Regex FullNameRegex();

    #endregion
}