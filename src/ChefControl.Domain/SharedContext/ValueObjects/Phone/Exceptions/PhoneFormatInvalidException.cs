using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.ValueObjects.Phone.Exceptions;

public class PhoneFormatInvalidException(string message)
    : DomainException(message);