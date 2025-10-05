namespace Riber.Domain.Exceptions;

public sealed class InvalidImageException(string message)
    : DomainException(message);