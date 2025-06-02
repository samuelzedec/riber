using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.CompanyContext.ValueObjects.CompanyName.Exceptions;

public class InvalidTradingNameException(string message) 
    : DomainException(message);