using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.Services.DocumentValidation.Exceptions;

public class InvalidLengthCpfException(string message) 
    : DomainException(message);