using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.Services.DocumentValidation.Exceptions;

public class UnsupportedCompanyTypeException(string message) 
    : DomainException(message);