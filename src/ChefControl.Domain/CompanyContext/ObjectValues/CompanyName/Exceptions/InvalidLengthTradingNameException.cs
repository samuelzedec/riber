using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.CompanyContext.ObjectValues.CompanyName.Exceptions;

public class InvalidTradingLengthNameException(string message) 
    : DomainException(message);