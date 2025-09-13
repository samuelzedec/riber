using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.Discount.Exceptions;

public sealed class InvalidFixedAmountException(string message)
    : DomainException(message);