using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.CompanyName.Exceptions;

public class EmptyNameCorporateException(string message) 
    : DomainException(message);