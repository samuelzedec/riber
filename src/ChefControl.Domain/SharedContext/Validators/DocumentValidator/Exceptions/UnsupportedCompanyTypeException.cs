using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.Validators.DocumentValidator.Exceptions;

public class UnsupportedCompanyTypeException(string message) 
    : DomainException(message);