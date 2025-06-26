using SnackFlow.Domain.SharedContext.Exceptions;

namespace SnackFlow.Domain.CompanyContext.ValueObjects.CompanyName.Exceptions;

public class InvalidTradingNameException(string message) 
    : DomainException(message);