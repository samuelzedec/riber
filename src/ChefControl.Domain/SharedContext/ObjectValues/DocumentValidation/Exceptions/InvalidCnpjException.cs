using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.ObjectValues.DocumentValidation.Exceptions;

public class InvalidCnpjException(string message) 
    : DomainException(message);