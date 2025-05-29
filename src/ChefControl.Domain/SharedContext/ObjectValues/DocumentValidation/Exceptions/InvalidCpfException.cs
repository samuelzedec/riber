using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.ObjectValues.DocumentValidation.Exceptions;

public class InvalidCpfException(string message) 
    : DomainException(message);