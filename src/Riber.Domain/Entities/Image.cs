using System.Security.Cryptography;
using System.Text;
using Riber.Domain.Constants.Messages.Entities;
using Riber.Domain.Exceptions;

namespace Riber.Domain.Entities;

public sealed class Image : BaseEntity
{
    #region Private properties

    private static readonly HashSet<string> AllowedImageTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/png",
        "image/jpg", 
        "image/jpeg",
        "image/webp"
    };

    #endregion

    #region Properties

    public bool ShouldDelete { get; private set; }
    public long Length { get; private init; }
    public string ContentType { get; private init; }
    public DateTimeOffset? MarkedForDeletionAt { get; private set; }
    public string OriginalName { get; init; }
    public string Key { get; private init; }
    public string Extension { get; init; }
    
    #endregion

    #region Constructors

    private Image() : base(Guid.Empty)
    {
        Length = 0;
        ContentType = string.Empty;
        OriginalName = string.Empty;
        Key = string.Empty;
        Extension = string.Empty;
        ShouldDelete = false;
        MarkedForDeletionAt = null;
    }
    private Image(long length, string contentType, string originalName, string key, string extension)
        : base(Guid.CreateVersion7())
    {
        Length = length;
        ContentType = contentType;
        OriginalName = originalName;
        Key = key;
        Extension = extension;
        ShouldDelete = false;
    }

    #endregion

    #region Factories

    public static Image Create(long length, string contentType, string originalName)
    {
        if (length <= 0)
            throw new InvalidLengthImageException(ImageErrors.Length);

        if (!IsValidImageType(contentType))
            throw new InvalidTypeImageException(ImageErrors.Type);

        if (string.IsNullOrWhiteSpace(originalName))
            throw new InvalidImageException(ImageErrors.NameEmpty);

        var extension = Path.GetExtension(originalName);
        if (string.IsNullOrEmpty(extension))
            throw new InvalidImageException(ImageErrors.ExtensionEmpty);
        
        var fileNameInBytes = Encoding.UTF8.GetBytes(originalName);
        var hashBytes = SHA256.HashData(fileNameInBytes);
        var key = Convert.ToHexString(hashBytes).ToLowerInvariant();

        return new Image(length, contentType, originalName, key, extension);
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

    public static bool IsValidImageType(string contentType) 
        => AllowedImageTypes.Contains(contentType);

    #endregion

    #region Operators

    public static implicit operator string(Image image)
        => image.ToString();

    #endregion

    #region Overrides

    public override string ToString() 
        => $"{Key}{Extension}";

    #endregion
}