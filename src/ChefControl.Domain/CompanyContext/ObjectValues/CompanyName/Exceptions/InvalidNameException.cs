using ChefControl.Domain.Shared.Exceptions;

namespace ChefControl.Domain.Companies.ObjectValues.CompanyName.Exceptions;

public class InvalidNameException(string message) 
    : DomainException(message);