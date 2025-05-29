namespace ChefControl.Domain.SharedContext.Exceptions;

public class DomainException(string message) 
    : Exception(message);