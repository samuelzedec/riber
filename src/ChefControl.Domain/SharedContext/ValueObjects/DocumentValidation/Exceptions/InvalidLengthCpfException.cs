using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.ValueObjects.DocumentValidation.Exceptions;

public class InvalidLengthCpfException(string message) 
    : DomainException(message);