using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.ValueObjects.Phone.Exceptions;

public class PhoneNullOrEmptyException(string message)
    : DomainException(message);