namespace SnackFlow.Domain.Exceptions;

public sealed class ProductDescriptionNullException(string message)
    : DomainException(message);