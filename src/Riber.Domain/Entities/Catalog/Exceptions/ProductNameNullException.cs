using Riber.Domain.Exceptions;

namespace Riber.Domain.Entities.Catalog.Exceptions;

public sealed class ProductNameNullException(string message)
    : DomainException(message);