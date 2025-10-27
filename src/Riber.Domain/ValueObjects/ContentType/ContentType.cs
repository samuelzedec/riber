using Riber.Domain.Constants.Messages.ValueObjects;
using Riber.Domain.ValueObjects.ContentType.Exceptions;

namespace Riber.Domain.ValueObjects.ContentType;

public sealed record ContentType : BaseValueObject
{
    #region Properties

    private static readonly HashSet<string> AllowedImageTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/png", "image/jpg", "image/jpeg", "image/webp"
    };

    public string Value { get; private set; } = string.Empty;

    #endregion

    #region Constructors

    private ContentType(string value)
        => Value = value;

    #endregion

    #region Factories

    public static ContentType Create(string value)
        => !IsValidImageType(value)
            ? throw new InvalidContentTypeException(ContentTypeErrors.Type)
            : new ContentType(value);

    #endregion

    #region Methods

    public static bool IsValidImageType(string contentType)
        => AllowedImageTypes.Contains(contentType);

    #endregion

    #region Operators

    public static implicit operator string(ContentType contentType)
        => contentType.ToString();

    #endregion

    #region Overrides

    public override string ToString()
        => Value;

    #endregion
}