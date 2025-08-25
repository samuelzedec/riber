using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.ValueObjects.Quantity.Exceptions;

public sealed class InvalidQuantityException(string message)
    : DomainException(message);