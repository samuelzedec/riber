using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.Services.DocumentValidation.Exceptions;

public class InvalidCnpjException(string message) 
    : DomainException(message);