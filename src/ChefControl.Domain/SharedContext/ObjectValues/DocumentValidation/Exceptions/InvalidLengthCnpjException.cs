using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.ObjectValues.DocumentValidation.Exceptions;

public class InvalidLengthCnpjException(string message) 
    : DomainException(message);