using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.Services.DocumentValidation.Exceptions;

public class InvalidLengthCnpjException(string message) 
    : DomainException(message);