using SnackFlow.Domain.SharedContext.Exceptions;

namespace SnackFlow.Domain.CompanyContext.ValueObjects.CompanyName.Exceptions;

public class InvalidTradingLengthNameException(string message) 
    : DomainException(message);