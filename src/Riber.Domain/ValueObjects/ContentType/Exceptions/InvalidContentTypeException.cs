using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.ContentType.Exceptions;

public sealed class InvalidTypeImageException(string message)
    : DomainException(message);