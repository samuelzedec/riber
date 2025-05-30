using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.ObjectValues.Email.Exceptions;

public class EmailNullOrEmptyException(string message) 
    : DomainException(message);