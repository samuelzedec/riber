using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.ContentType.Exceptions;

public sealed class InvalidContentTypeException(string message)
    : DomainException(message);