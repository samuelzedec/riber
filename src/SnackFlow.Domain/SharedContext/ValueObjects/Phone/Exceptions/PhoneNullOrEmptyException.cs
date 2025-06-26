using SnackFlow.Domain.SharedContext.Exceptions;

namespace SnackFlow.Domain.SharedContext.ValueObjects.Phone.Exceptions;

public class PhoneNullOrEmptyException(string message)
    : DomainException(message);