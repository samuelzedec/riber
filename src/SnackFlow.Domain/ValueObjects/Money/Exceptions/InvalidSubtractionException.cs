using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.ValueObjects.Money.Exceptions;

public sealed class InvalidSubtractionException(string message)
    : DomainException(message);