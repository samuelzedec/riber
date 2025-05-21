using ChefControl.Domain.Shared.Exceptions;

namespace ChefControl.Domain.Shared.ObjectValues.DocumentValidation.Exceptions;

public class InvalidCnpjException(string message) 
    : DomainException(message);