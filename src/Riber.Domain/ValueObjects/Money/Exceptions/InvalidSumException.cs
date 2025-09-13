using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.Money.Exceptions;

public sealed class InvalidSumException(string message)
    : DomainException(message);