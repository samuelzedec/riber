using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.Money.Exceptions;

public sealed class InvalidMoneyException(string message)
    : DomainException(message);