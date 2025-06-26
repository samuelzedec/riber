using SnackFlow.Domain.SharedContext.Exceptions;

namespace SnackFlow.Domain.SharedContext.ValueObjects.Email.Exceptions;

public class EmailNullOrEmptyException(string message) 
    : DomainException(message);