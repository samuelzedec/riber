using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.ValueObjects.Money.Exceptions;

public sealed class InvalidMoneyException(string message)
    : DomainException(message);