using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.CompanyName.Exceptions;

public class InvalidFantasyNameException(string message) 
    : DomainException(message);