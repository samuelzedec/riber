using Riber.Domain.Exceptions;

namespace Riber.Domain.Entities.Catalog.Exceptions;

public sealed class InvalidLengthImageException(string message)
    : DomainException(message);
