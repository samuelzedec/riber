using Riber.Domain.Exceptions;

namespace Riber.Domain.Entities.Catalog.Exceptions;

public sealed class ProductCategoryNameNullException(string message)
    : DomainException(message);