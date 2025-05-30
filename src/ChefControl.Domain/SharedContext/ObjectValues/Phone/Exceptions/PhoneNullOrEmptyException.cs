using ChefControl.Domain.SharedContext.Exceptions;

namespace ChefControl.Domain.SharedContext.ObjectValues.Phone.Exceptions;

public class PhoneNullOrEmptyException(string message)
    : DomainException(message);