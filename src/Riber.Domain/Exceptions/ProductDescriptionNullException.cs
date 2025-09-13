namespace Riber.Domain.Exceptions;

public sealed class ProductDescriptionNullException(string message)
    : DomainException(message);