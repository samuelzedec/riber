using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.ValueObjects.Money.Exceptions;

public sealed class InvalidSumException(string message)
    : DomainException(message);