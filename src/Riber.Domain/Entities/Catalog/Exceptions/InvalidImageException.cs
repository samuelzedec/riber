using Riber.Domain.Exceptions;

namespace Riber.Domain.Entities.Catalog.Exceptions;

public sealed class InvalidImageException(string message)
    : DomainException(message);