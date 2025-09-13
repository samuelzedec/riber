using Riber.Domain.Exceptions;

namespace Riber.Domain.Validators.DocumentValidator.Exceptions;

public class InvalidCnpjException(string message) 
    : DomainException(message);