using Riber.Domain.Exceptions;

namespace Riber.Domain.Entities.Catalog.Exceptions;

public sealed class EmptyProductCategoryNameException(string message)
    : DomainException(message);