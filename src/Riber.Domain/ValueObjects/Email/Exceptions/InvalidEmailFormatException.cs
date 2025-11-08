using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.Email.Exceptions;

public class InvalidEmailFormatException(string message) 
    : DomainException(message);