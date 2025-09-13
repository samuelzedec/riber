using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.Phone.Exceptions;

public class PhoneNullOrEmptyException(string message)
    : DomainException(message);