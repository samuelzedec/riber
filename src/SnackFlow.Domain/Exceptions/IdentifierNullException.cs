namespace SnackFlow.Domain.Exceptions;

public sealed class IdentifierNullException(string message)
    : DomainException(message);