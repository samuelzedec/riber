using Riber.Domain.Exceptions;

namespace Riber.Domain.Entities.Catalog.Exceptions;

public sealed class EmptyProductNameException(string message)
    : DomainException(message);