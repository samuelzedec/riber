using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.ValueObjects.Discount.Exceptions;

public sealed class InvalidPercentageException(string message)
    : DomainException(message);