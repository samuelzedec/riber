using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.ValueObjects.DocumentValidation.Exceptions;

public class InvalidCnpjException(string message) 
    : DomainException(message);