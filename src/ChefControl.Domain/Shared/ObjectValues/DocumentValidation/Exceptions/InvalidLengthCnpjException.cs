using ChefControl.Domain.Shared.Exceptions;

namespace ChefControl.Domain.Shared.ObjectValues.DocumentValidation.Exceptions;

public class InvalidLengthCnpjException(string message) 
    : DomainException(message);