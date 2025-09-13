using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.Quantity.Exceptions;

public sealed class InvalidQuantityException(string message)
    : DomainException(message);