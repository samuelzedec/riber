using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.ValueObjects.CompanyName.Exceptions;

public class InvalidNameCorporateException(string message) 
    : DomainException(message);