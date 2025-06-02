using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.ValueObjects.Email.Exceptions;

public class EmailFormatInvalidException(string message) 
    : DomainException(message);