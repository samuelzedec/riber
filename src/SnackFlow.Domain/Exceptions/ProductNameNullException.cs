namespace SnackFlow.Domain.Exceptions;

public sealed class ProductNameNullException(string message)
    : DomainException(message);