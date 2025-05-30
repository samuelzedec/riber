using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.ObjectValues.Phone.Exceptions;

public class PhoneFormatInvalidException(string message)
    : DomainException(message);