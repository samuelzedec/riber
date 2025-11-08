using Riber.Domain.Exceptions;

namespace Riber.Domain.Entities.Catalog.Exceptions;

public sealed class EmptyProductDescriptionException(string message)
    : DomainException(message);