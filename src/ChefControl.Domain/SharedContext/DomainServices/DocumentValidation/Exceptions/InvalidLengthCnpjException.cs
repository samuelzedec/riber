using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.DomainServices.DocumentValidation.Exceptions;

public class InvalidLengthCnpjException(string message) 
    : DomainException(message);