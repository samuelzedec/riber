using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.Money.Exceptions;

public sealed class CurrencyMismatchException(string message)
    : DomainException(message);