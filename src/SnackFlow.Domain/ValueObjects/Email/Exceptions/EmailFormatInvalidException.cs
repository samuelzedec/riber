using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.ValueObjects.Email.Exceptions;

public class EmailFormatInvalidException(string message) 
    : DomainException(message);