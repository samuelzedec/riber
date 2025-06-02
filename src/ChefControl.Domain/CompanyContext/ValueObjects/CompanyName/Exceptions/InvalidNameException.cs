using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.CompanyContext.ValueObjects.CompanyName.Exceptions;

public class InvalidNameException(string message) 
    : DomainException(message);