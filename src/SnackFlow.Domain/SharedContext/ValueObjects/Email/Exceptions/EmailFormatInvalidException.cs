using SnackFlow.Domain.SharedContext.Exceptions;

namespace SnackFlow.Domain.SharedContext.ValueObjects.Email.Exceptions;

public class EmailFormatInvalidException(string message) 
    : DomainException(message);