namespace Riber.Domain.Exceptions;

public sealed class InvalidLengthImageException(string message)
    : DomainException(message);
