using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.CompanyContext.ObjectValues.CompanyName.Exceptions;

public class InvalidTradingNameException(string message) 
    : DomainException(message);