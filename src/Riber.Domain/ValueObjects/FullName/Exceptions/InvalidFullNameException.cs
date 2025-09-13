using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.FullName.Exceptions;

public sealed class InvalidFullNameException(string message) 
    : DomainException(message);