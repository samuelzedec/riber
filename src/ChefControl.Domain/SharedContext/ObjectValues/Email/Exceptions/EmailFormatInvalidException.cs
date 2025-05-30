using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.ObjectValues.Email.Exceptions;

public class EmailFormatInvalidException(string message) 
    : DomainException(message);