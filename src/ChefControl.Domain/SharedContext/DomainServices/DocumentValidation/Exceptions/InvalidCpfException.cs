using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.DomainServices.DocumentValidation.Exceptions;

public class InvalidCpfException(string message) 
    : DomainException(message);