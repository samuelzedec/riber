using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.CompanyName.Exceptions;

public class InvalidNameCorporateException(string message) 
    : DomainException(message);