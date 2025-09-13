using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.CompanyName.Exceptions;

public class InvalidLengthCorporateNameException(string message)
    : DomainException(message);