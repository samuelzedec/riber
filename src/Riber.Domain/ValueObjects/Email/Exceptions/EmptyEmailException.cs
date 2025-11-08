using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.Email.Exceptions;

public class EmptyEmailException(string message) 
    : DomainException(message);