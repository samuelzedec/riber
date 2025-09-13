namespace Riber.Domain.Exceptions;

public sealed class ProductCategoryCodeNullException(string message)
    : DomainException(message);