using ChefControl.Domain.Shared.Exceptions;

namespace ChefControl.Domain.Shared.ObjectValues.DocumentValidation.Exceptions;

public class UnsupportedCompanyTypeException(string message) 
    : DomainException(message);