using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.CompanyName.Exceptions;

public class EmptyFantasyNameException(string message) 
    : DomainException(message);