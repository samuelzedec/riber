using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.CompanyName.Exceptions;

public class InvalidCorporateNameLengthException(string message)
    : DomainException(message);