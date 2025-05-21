using ChefControl.Domain.Shared.Exceptions;

namespace ChefControl.Domain.Companies.ObjectValues.CompanyName.Exceptions;

public class InvalidTradingLengthNameException(string message) 
    : DomainException(message);