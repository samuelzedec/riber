using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.ValueObjects.FullName.Exceptions;

public sealed class InvalidFullNameException(string message) 
    : DomainException(message);