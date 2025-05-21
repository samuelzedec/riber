using ChefControl.Domain.Shared.Exceptions;

namespace ChefControl.Domain.Companies.ObjectValues.CompanyName.Exceptions;

public class InvalidLengthNameException(string message)
    : DomainException(message);