using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.ValueObjects.FullName.Exceptions;

public sealed class InvalidLengthFullNameException(string message) 
    : DomainException(message);