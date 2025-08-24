using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.ValueObjects.Discount.Exceptions;

public sealed class InvalidDiscountException(string message)
    : DomainException(message);