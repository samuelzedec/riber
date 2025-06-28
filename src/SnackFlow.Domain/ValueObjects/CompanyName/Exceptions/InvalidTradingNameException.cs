using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.ValueObjects.CompanyName.Exceptions;

public class InvalidTradingNameException(string message) 
    : DomainException(message);