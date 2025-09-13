namespace Riber.Domain.Exceptions;

public sealed class ProductCategoryNameNullException(string message)
    : DomainException(message);