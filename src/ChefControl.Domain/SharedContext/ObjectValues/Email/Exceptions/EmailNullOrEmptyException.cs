namespace ChefControl.Domain.SharedContext.ObjectValues.Email.Exceptions;

public class EmailNullOrEmptyException(string message) 
    : Exception(message);