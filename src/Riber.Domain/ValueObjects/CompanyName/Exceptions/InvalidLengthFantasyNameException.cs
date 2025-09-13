using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.CompanyName.Exceptions;

public class InvalidFantasyLengthNameException(string message) 
    : DomainException(message);