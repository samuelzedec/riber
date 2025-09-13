using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.Email.Exceptions;

public class EmailFormatInvalidException(string message) 
    : DomainException(message);