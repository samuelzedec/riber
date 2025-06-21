using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.Validators.DocumentValidator.Exceptions;

public class InvalidCpfException(string message) 
    : DomainException(message);