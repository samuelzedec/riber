using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.ValueObjects.DocumentValidation.Exceptions;

public class UnsupportedCompanyTypeException(string message) 
    : DomainException(message);