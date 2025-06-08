using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.ValueObjects.DocumentValidation.Exceptions;

public class InvalidCpfException(string message) 
    : DomainException(message);