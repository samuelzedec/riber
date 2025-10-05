namespace Riber.Domain.Exceptions;

public sealed class InvalidTypeImageException(string message)
    : DomainException(message);