using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.Validators.DocumentValidator.Exceptions;

public class InvalidCnpjException(string message) 
    : DomainException(message);