using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.Phone.Exceptions;

public class EmptyPhoneException(string message)
    : DomainException(message);