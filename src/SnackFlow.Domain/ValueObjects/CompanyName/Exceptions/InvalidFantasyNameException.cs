using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.ValueObjects.CompanyName.Exceptions;

public class InvalidFantasyNameException(string message) 
    : DomainException(message);