using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.ValueObjects.Email.Exceptions;

public class EmailNullOrEmptyException(string message) 
    : DomainException(message);