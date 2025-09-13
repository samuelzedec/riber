using Riber.Domain.Exceptions;

namespace Riber.Domain.Validators.DocumentValidator.Exceptions;

public class InvalidLengthCnpjException(string message) 
    : DomainException(message);