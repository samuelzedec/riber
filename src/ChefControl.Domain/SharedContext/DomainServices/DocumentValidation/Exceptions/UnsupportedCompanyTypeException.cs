using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.DomainServices.DocumentValidation.Exceptions;

public class UnsupportedCompanyTypeException(string message) 
    : DomainException(message);