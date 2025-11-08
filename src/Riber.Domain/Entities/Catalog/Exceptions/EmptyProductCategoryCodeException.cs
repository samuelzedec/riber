using Riber.Domain.Exceptions;

namespace Riber.Domain.Entities.Catalog.Exceptions;

public sealed class EmptyProductCategoryCodeException(string message)
    : DomainException(message);