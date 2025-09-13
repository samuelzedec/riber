using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.Discount.Exceptions;

public sealed class InvalidPercentageException(string message)
    : DomainException(message);