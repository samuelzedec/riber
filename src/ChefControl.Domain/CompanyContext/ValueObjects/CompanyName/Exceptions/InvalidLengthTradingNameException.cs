using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.CompanyContext.ValueObjects.CompanyName.Exceptions;

public class InvalidTradingLengthNameException(string message) 
    : DomainException(message);