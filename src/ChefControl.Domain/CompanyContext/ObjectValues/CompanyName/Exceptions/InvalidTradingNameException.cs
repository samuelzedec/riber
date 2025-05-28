using ChefControl.Domain.Shared.Exceptions;

namespace ChefControl.Domain.Companies.ObjectValues.CompanyName.Exceptions;

public class InvalidTradingNameException(string message) 
    : DomainException(message);