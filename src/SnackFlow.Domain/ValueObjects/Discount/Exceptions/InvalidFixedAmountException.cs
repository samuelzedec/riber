using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.ValueObjects.Discount.Exceptions;

public sealed class InvalidFixedAmountException(string message)
    : DomainException(message);