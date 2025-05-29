using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.CompanyContext.ObjectValues.CompanyName.Exceptions;

public class InvalidNameException(string message) 
    : DomainException(message);