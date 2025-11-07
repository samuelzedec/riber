using Riber.Domain.Exceptions;

namespace Riber.Domain.Entities.Catalog.Exceptions;

public sealed class ImageIsEmptyException(string message)
    : DomainException(message);
