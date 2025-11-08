using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.CompanyName.Exceptions;

public class InvalidFantasyNameLengthException(string message) 
    : DomainException(message);