﻿using System.Text.RegularExpressions;
using Riber.Domain.Constants.Messages.ValueObjects;
using Riber.Domain.ValueObjects.Phone.Exceptions;

namespace Riber.Domain.ValueObjects.Phone;

public sealed partial record Phone : BaseValueObject
{
    #region Properties

    public string Value { get; }

    #endregion

    #region Constants

    public const string RegexPattern = @"^(?:\(?(?:11|[12-9][0-9])\)?\s?)?9[0-9]{4}[\s\-]?[0-9]{4}$";

    #endregion

    #region Constructors

    private Phone(string value)
        => Value = value;

    #endregion

    #region Factories

    public static Phone Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new PhoneNullOrEmptyException(PhoneErrors.Empty);

        value = value.Trim();

        return PhoneRegex().IsMatch(value)
            ? new Phone(RemoveFormatting(value))
            : throw new PhoneFormatInvalidException(PhoneErrors.Format);
    }

    #endregion

    #region Source Generator

    [GeneratedRegex(RegexPattern)]
    public static partial Regex PhoneRegex();

    #endregion

    #region Operators

    public static implicit operator string(Phone phone)
        => phone.ToString();

    #endregion

    #region Overrides

    public override string ToString()
        => Value.Length == 11
            ? $"({Value[..2]}) {Value[2..7]}-{Value[7..]}"
            : $"({Value[..2]}) {Value[2..6]}-{Value[6..]}";

    #endregion

    #region Public Methods

    public static string RemoveFormatting(string value)
        => new([.. value.Where(char.IsDigit)]);

    #endregion
}