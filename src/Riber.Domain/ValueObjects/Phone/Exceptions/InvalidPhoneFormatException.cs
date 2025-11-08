using Riber.Domain.Exceptions;

namespace Riber.Domain.ValueObjects.Phone.Exceptions;

public class InvalidPhoneFormatException(string message)
    : DomainException(message);