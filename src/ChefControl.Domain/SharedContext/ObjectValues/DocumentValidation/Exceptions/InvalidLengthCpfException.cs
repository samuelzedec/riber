using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.ObjectValues.DocumentValidation.Exceptions;

public class InvalidLengthCpfException(string message) 
    : DomainException(message);