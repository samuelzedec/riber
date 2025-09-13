using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.Money.Exceptions;

public sealed class InvalidSubtractionException(string message)
    : DomainException(message);