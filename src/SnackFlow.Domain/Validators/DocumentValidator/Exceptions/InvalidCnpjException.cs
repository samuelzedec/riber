using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.Validators.DocumentValidator.Exceptions;

public class InvalidCnpjException(string message) 
    : DomainException(message);