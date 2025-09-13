using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.Phone.Exceptions;

public class PhoneFormatInvalidException(string message)
    : DomainException(message);