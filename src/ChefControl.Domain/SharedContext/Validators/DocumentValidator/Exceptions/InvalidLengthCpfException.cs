using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.Validators.DocumentValidator.Exceptions;

public class InvalidLengthCpfException(string message) 
    : DomainException(message);