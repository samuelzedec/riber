using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.Email.Exceptions;

public class EmailNullOrEmptyException(string message) 
    : DomainException(message);