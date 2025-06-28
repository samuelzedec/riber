using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.ValueObjects.Phone.Exceptions;

public class PhoneFormatInvalidException(string message)
    : DomainException(message);