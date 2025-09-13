using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.FullName.Exceptions;

public sealed class InvalidLengthFullNameException(string message) 
    : DomainException(message);