using SnackFlow.Domain.SharedContext.Exceptions;

namespace SnackFlow.Domain.SharedContext.Validators.DocumentValidator.Exceptions;

public class InvalidCnpjException(string message) 
    : DomainException(message);