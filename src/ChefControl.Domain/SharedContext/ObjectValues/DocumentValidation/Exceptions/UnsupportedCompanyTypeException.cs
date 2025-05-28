using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.ObjectValues.DocumentValidation.Exceptions;

public class UnsupportedCompanyTypeException(string message) 
    : DomainException(message);