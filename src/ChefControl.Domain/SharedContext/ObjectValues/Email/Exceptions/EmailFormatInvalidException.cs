namespace ChefControl.Domain.SharedContext.ObjectValues.Email.Exceptions;

public class EmailFormatInvalidException(string message) 
    : Exception(message);