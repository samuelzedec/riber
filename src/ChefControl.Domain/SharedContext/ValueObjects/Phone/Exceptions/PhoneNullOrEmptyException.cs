using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.ValueObjects.Phone.Exceptions;

public class PhoneNullOrEmptyException(string message)
    : DomainException(message);