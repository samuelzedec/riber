using Riber.Domain.Abstractions.ValueObjects;
using Riber.Domain.Constants.Messages.Entities;
using Riber.Domain.Entities.Abstractions;
using Riber.Domain.Entities.Catalog.Exceptions;
using Riber.Domain.Exceptions;
using Riber.Domain.ValueObjects.ContentType;

namespace Riber.Domain.Entities.Catalog;

public sealed class Image : BaseEntity, IHasContentType
{
    #region Properties

    public bool ShouldDelete { get; private set; }
    public long Length { get; private init; }
    public ContentType ContentType { get; init; }
    public DateTimeOffset? MarkedForDeletionAt { get; private set; }
    public string OriginalName { get; init; }
    public string Extension { get; init; }

    #endregion

    #region Constructors

#pragma warning disable CS8618, CA1823
    private Image() : base(Guid.Empty) { }
#pragma warning restore CS8618, CA1823

    private Image(long length, string contentType, string originalName, string extension)
        : base(Guid.CreateVersion7())
    {
        Length = length;
        ContentType = ContentType.Create(contentType);
        OriginalName = originalName;
        Extension = extension;
        ShouldDelete = false;
    }

    #endregion

    #region Factories

    public static Image Create(long length, string originalName, string contentType)
    {
        if (length <= 0)
            throw new InvalidLengthImageException(ImageErrors.Length);

        if (string.IsNullOrWhiteSpace(originalName))
            throw new InvalidImageException(ImageErrors.NameEmpty);

        var extension = Path.GetExtension(originalName);
        return string.IsNullOrEmpty(extension)
            ? throw new InvalidImageException(ImageErrors.ExtensionEmpty)
            : new Image(length, contentType, originalName, extension);
    }

    #endregion

    #region Methods

    public void MarkForDeletion()
    {
        ShouldDelete = true;
        MarkedForDeletionAt = DateTimeOffset.UtcNow;
    }

    public bool IsMarkedForDeletion()
        => ShouldDelete && MarkedForDeletionAt.HasValue;

    #endregion

    #region Operators

    public static implicit operator string(Image image)
        => image.ToString();

    #endregion

    #region Overrides

    public override string ToString()
        => $"{Id}{Extension}";

    #endregion
}