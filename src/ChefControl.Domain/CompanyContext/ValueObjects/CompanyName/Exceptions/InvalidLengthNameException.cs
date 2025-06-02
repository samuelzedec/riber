using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.CompanyContext.ValueObjects.CompanyName.Exceptions;

public class InvalidLengthNameException(string message)
    : DomainException(message);