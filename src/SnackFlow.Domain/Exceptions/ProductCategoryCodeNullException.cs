namespace SnackFlow.Domain.Exceptions;

public sealed class ProductCategoryCodeNullException(string message)
    : DomainException(message);