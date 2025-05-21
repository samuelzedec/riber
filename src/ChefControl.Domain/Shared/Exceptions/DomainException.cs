namespace ChefControl.Domain.Shared.Exceptions;

public class DomainException(string message) 
    : Exception(message);