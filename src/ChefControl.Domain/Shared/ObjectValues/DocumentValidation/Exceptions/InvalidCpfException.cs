using ChefControl.Domain.Shared.Exceptions;

namespace ChefControl.Domain.Shared.ObjectValues.DocumentValidation.Exceptions;

public class InvalidCpfException(string message) 
    : DomainException(message);