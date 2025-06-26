using SnackFlow.Domain.SharedContext.Exceptions;

namespace SnackFlow.Domain.SharedContext.ValueObjects.Phone.Exceptions;

public class PhoneFormatInvalidException(string message)
    : DomainException(message);