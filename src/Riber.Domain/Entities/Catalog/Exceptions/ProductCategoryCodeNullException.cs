using Riber.Domain.Exceptions;

namespace Riber.Domain.Entities.Catalog.Exceptions;

public sealed class ProductCategoryCodeNullException(string message)
    : DomainException(message);