using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.ValueObjects.Email.Exceptions;

public class EmailNullOrEmptyException(string message) 
    : DomainException(message);