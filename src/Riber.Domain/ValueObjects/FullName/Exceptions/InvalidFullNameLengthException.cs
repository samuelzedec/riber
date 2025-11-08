using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.FullName.Exceptions;

public sealed class InvalidFullNameLengthException(string message) 
    : DomainException(message);