using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.CompanyContext.ObjectValues.CompanyName.Exceptions;

public class InvalidLengthNameException(string message)
    : DomainException(message);