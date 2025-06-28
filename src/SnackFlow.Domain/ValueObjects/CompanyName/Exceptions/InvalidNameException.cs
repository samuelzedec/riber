using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.ValueObjects.CompanyName.Exceptions;

public class InvalidNameException(string message) 
    : DomainException(message);