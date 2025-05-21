using ChefControl.Domain.Shared.Exceptions;

namespace ChefControl.Domain.Shared.ObjectValues.DocumentValidation.Exceptions;

public class InvalidLengthCpfException(string message) 
    : DomainException(message);