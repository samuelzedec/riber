using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.Services.DocumentValidation.Exceptions;

public class InvalidCpfException(string message) 
    : DomainException(message);