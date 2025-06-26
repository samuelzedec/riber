using SnackFlow.Domain.SharedContext.Exceptions;

namespace SnackFlow.Domain.CompanyContext.ValueObjects.CompanyName.Exceptions;

public class InvalidNameException(string message) 
    : DomainException(message);