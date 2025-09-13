using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.Discount.Exceptions;

public sealed class InvalidDiscountException(string message)
    : DomainException(message);